#include "open62541.h"
#include <stdio.h>
#include <signal.h>
#include "globalVariables.h"
#include "manager.c"

UA_Boolean running = true;
static void stopHandler(int sig) {
    UA_LOG_INFO(UA_Log_Stdout, UA_LOGCATEGORY_USERLAND, "received ctrl-c");
    running = false;
}

int main(int argc, char *argv[]) {
    signal(SIGINT, stopHandler);
    signal(SIGTERM, stopHandler);

	if (argc != 1)
	{
		printf("Too many arguments!\n");
		return -1;
	}

	for(int index = 0; index < numberOfReferences; index++)
	{
		char* serverAddress          = SERVER_APP_URL[index];
		client[index] = UA_Client_new();
		UA_ClientConfig_setDefault(UA_Client_getConfig(client[index]));

		UA_StatusCode connectStatus  = UA_Client_connect(client[index], serverAddress);

		if (connectStatus != UA_STATUSCODE_GOOD)
		{
			printf("could not connect to server: %s!\n", SERVER_APP_URL[index]);
			return -1;
		}
	}

	manageTank(client[1], client[0], true);

	for(int index = 0; index < numberOfReferences; index++)
	{ 
           UA_Client_delete(client[index]);
	}
	
	return 0;
}
