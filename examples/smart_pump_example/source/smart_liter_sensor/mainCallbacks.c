/* 
 * This is an automatically generated file.
 */

#include "open62541.h"
#include "tankSimulation.c"

// ##### OPC UA Variable callback example #####
// The OPC UA provides value callback. Thanks to value callback OPC UA Variables the user can bind real data and update it with every value change.
// An example of OPC UA Variable callback is given below:
// callback function
static void ReadExample(UA_Server *server, const UA_NodeId *sessionId, void *sessionContext, const UA_NodeId *nodeid, void *nodeContext, const UA_NumericRange *range, const UA_DataValue *data)
{
	UA_UInt32 value = getLiquidLevel();
	UA_Variant myVar;
	UA_Variant_setScalar(&myVar, &value, &UA_TYPES[UA_TYPES_UINT32]);

	UA_NodeId currentNodeId = UA_NODEID_NUMERIC(3, 1001);

	UA_Server_writeValue(server, currentNodeId, myVar);
}

// adding callback to server
static void ValueCallbackExample(UA_Server *server)
{
	tankInit();

	UA_NodeId currentNodeId = UA_NODEID_NUMERIC(3, 1001);

	UA_ValueCallback callback;
	callback.onRead = ReadExample;
	callback.onWrite = NULL;

	UA_Server_setVariableNode_valueCallback(server, currentNodeId, callback);
}

// ############################################


UA_StatusCode addCallbacks(UA_Server *server)
{
	// ##### OPC UA Variable callback example #####
	ValueCallbackExample(server);

	return UA_STATUSCODE_GOOD;
}
