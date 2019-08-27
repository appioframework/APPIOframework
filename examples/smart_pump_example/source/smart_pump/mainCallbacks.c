#include "open62541.h"
#include "pumpSimulation.c"

// volumeStream callback function
static void volumeStreamReadValue(UA_Server *server, const UA_NodeId *sessionId, void *sessionContext, const UA_NodeId *nodeid, void *nodeContext, const UA_NumericRange *range, const UA_DataValue *data)
{
    UA_UInt32 value = gVolumeStream;
    UA_Variant myVar;
    UA_Variant_setScalar(&myVar, &value, &UA_TYPES[UA_TYPES_UINT32]);

    UA_NodeId currentNodeId = UA_NODEID_NUMERIC(3, 1001);

    UA_Server_writeValue(server, currentNodeId, myVar);
}

// adding callback to volumeStream
static void addValueCallbackToVolumeStreamVariable(UA_Server *server)
{
    UA_NodeId currentNodeId = UA_NODEID_NUMERIC(3, 1001);

    UA_ValueCallback callback;
    callback.onRead = volumeStreamReadValue;
    callback.onWrite = NULL;

    UA_Server_setVariableNode_valueCallback(server, currentNodeId, callback);

}

// callback of startPump Method
UA_MethodCallback startPumpCallback(UA_Server *server, const UA_NodeId *sessionId, void *sessionContext, const UA_NodeId *methodId, void *methodContext, const UA_NodeId *objectId, void *objectContext, size_t inputSize, const UA_Variant *input, size_t outputSize, UA_Variant *output)
{
    startPump();
    return UA_STATUSCODE_GOOD;
}

// callback of stopPump Method
UA_MethodCallback stopPumpCallback(UA_Server *server, const UA_NodeId *sessionId, void *sessionContext, const UA_NodeId *methodId, void *methodContext, const UA_NodeId *objectId, void *objectContext, size_t inputSize, const UA_Variant *input, size_t outputSize, UA_Variant *output)
{
    stopPump();
    return UA_STATUSCODE_GOOD;
}

// callback of setVolumeStream Method
UA_MethodCallback setVolumeStreamCallback(UA_Server *server, const UA_NodeId *sessionId, void *sessionContext, const UA_NodeId *methodId, void *methodContext, const UA_NodeId *objectId, void *objectContext, size_t inputSize, const UA_Variant *input, size_t outputSize, UA_Variant *output)
{
    setVolumeStream(*((UA_UInt32*)input->data));
    return UA_STATUSCODE_GOOD;
}

// setting callbacks
UA_StatusCode addCallbacks(UA_Server *server)
{
	// initialize simulation text file
	updateFile();

    addValueCallbackToVolumeStreamVariable(server);
    // startPump
    if(UA_Server_setMethodNode_callback(server, UA_NODEID_NUMERIC(3, 3000), startPumpCallback) != UA_STATUSCODE_GOOD)
    {
        return UA_STATUSCODE_BADUNEXPECTEDERROR;
    }

    // stopPump
    if(UA_Server_setMethodNode_callback(server, UA_NODEID_NUMERIC(3, 3001), stopPumpCallback) != UA_STATUSCODE_GOOD)
    {
        return UA_STATUSCODE_BADUNEXPECTEDERROR;
    }

    // setVolumeStream
    if(UA_Server_setMethodNode_callback(server, UA_NODEID_NUMERIC(3, 3002), setVolumeStreamCallback) != UA_STATUSCODE_GOOD)
    {
        return UA_STATUSCODE_BADUNEXPECTEDERROR;
    }

    return UA_STATUSCODE_GOOD;
}
