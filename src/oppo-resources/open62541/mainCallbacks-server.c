/* 
 * This is an automatically generated file.
 */
#include "open62541.h"

/*
// ##### OPC UA Variable callback example #####
// The OPC UA provides value callback. Thanks to value callback OPC UA Variables the user can bind real data and update it with every value change.
// An example of OPC UA Variable callback is given below:
// callback function
static void ReadExample(UA_Server *server, const UA_NodeId *sessionId, void *sessionContext, const UA_NodeId *nodeid, void *nodeContext, const UA_NumericRange *range, const UA_DataValue *data)
{
	UA_UInt32 value = // TODO: specify your real data variable // ;
	UA_Variant myVar;
	UA_Variant_setScalar(&myVar, &value, &UA_TYPES[// TODO: specify value UA datatype, e.g. UA_TYPES_UINT32 //]);

	UA_NodeId currentNodeId = // TODO: specify Id of the Node to store real data value, e.g. UA_NODEID_NUMERIC(1, 1000) //;

	UA_Server_writeValue(server, currentNodeId, myVar);
}

// adding callback to server
static void ValueCallbackExample(UA_Server *server)
{
	UA_NodeId currentNodeId = // TODO: specify Id of the Node to store real data value, e.g. UA_NODEID_NUMERIC(1, 1000) //;

	UA_ValueCallback callback;
	callback.onRead = ReadExample;
	callback.onWrite = NULL;

	UA_Server_setVariableNode_valueCallback(server, currentNodeId, callback);
}

// ############################################
*/

/*
// ##### OPC UA Method callback example #####
// The OPC UA provides method callback. Thanks to method callback the user can add own logic to OPC UA Method call.
// An eample of OPC UA Method callback is given below:
// callback function
UA_MethodCallback MethodCallbackExample(UA_Server *server, const UA_NodeId *sessionId, void *sessionContext, const UA_NodeId *methodId, void *methodContext, const UA_NodeId *objectId, void *objectContext, size_t inputSize, const UA_Variant *input, size_t outputSize, UA_Variant *output)
{
	// TODO: add your logic here //
	return UA_STATUSCODE_GOOD;
}

// ############################################
*/


UA_StatusCode addCallbacks(UA_Server *server)
{
	// ##### OPC UA Variable callback example #####
	//ValueCallbackExample(server);

	// ##### OPC UA Method callback example #####
	// setVolumeStream
	/*if(UA_Server_setMethodNode_callback(server, // TODO: specify Id of the Method to call, e.g. UA_NODEID_NUMERIC(1, 1000) //, MethodCallbackExample) != UA_STATUSCODE_GOOD)
	{
		return UA_STATUSCODE_BADUNEXPECTEDERROR;
	}*/

	return UA_STATUSCODE_GOOD;
}