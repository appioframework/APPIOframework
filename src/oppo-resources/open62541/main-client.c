#include "open62541.h"
#include <stdio.h>

int main(int argc, char *argv[]) {
	if (argc != 2)
	{
		printf("server URL is missing!\n");
		return -1;
	}

	char* serverAddress          = argv[1];

	UA_ClientConfig clientConfig = UA_ClientConfig_default;
	UA_Client* client            = UA_Client_new(clientConfig);

	UA_StatusCode connectStatus  = 
		UA_Client_connect(client, serverAddress);

	if (connectStatus != UA_STATUSCODE_GOOD)
	{
		printf("could not connect to server!\n");
		return -1;
	}

	UA_Variant value;
	UA_Variant_init(&value);

	const char* nodeName         = "temperature";

	UA_NodeId stringNode         = UA_NODEID_STRING(1, nodeName);
	UA_StatusCode readStatus     = UA_Client_readValueAttribute(client, stringNode, &value);

	if(readStatus == UA_STATUSCODE_GOOD && UA_Variant_hasScalarType(&value, &UA_TYPES[UA_TYPES_INT32]))
	{           
		printf("The value of node-id: %d and display-name: %s is: %d\n", stringNode.namespaceIndex, stringNode.identifier.string.data, *(UA_Int32*)value.data);
	}

	UA_Client_delete(client);

	return 0;
}