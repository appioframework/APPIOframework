#include "open62541.h"
#include <stdio.h>
#include <signal.h>
#include "globalVariables.h"

UA_Boolean running = true;
static void stopHandler(int sig) {
	UA_LOG_INFO(UA_Log_Stdout, UA_LOGCATEGORY_USERLAND, "received ctrl-c");
	running = false;
}

int main(int argc, char *argv[])
{
	signal(SIGINT, stopHandler);
	signal(SIGTERM, stopHandler);

	if (argc != 1)
	{
		printf("Too many arguments!\n");
		return -1;
	}

	for (int index = 0; index < numberOfReferences; index++)
	{
		char* serverAddress = SERVER_APP_URL[index];
		UA_ClientConfig clientConfig = UA_ClientConfig_default;
		client[index] = UA_Client_new(clientConfig);

		UA_StatusCode connectStatus = UA_Client_connect(client[index], serverAddress);

		if (connectStatus != UA_STATUSCODE_GOOD)
		{
			printf("could not connect to server: %s!\n", SERVER_APP_URL[index]);
			return -1;
		}
	}

	printf("Successfully connected with %d servers.\n", numberOfReferences);

	/* TODO: place your code here */

	for (int index = 0; index < numberOfReferences; index++)
	{
		UA_Client_delete(client[index]);
	}

	printf("All %d server connections were closed.\n", numberOfReferences);

	return 0;
}