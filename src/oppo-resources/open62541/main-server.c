#include "open62541.h"
#include <signal.h>

UA_Boolean running = true;
static void stopHandler(int sig) {
    UA_LOG_INFO(UA_Log_Stdout, UA_LOGCATEGORY_USERLAND, "received ctrl-c");
    running = false;
}

static void createTemperatureVariableNode(UA_Server* server) {
    const char* nodeName            = "temperature";
    const char* nodeLocale          = "en-US";

    UA_VariableAttributes attr      = UA_VariableAttributes_default;
    UA_Int32 temperatureValue       = 45;

    attr.displayName                = UA_LOCALIZEDTEXT_ALLOC(nodeLocale, nodeName);

    UA_Variant_setScalar(&attr.value, &temperatureValue, &UA_TYPES[UA_TYPES_INT32]);

    UA_NodeId temperatureNodeId     = UA_NODEID_STRING_ALLOC(1, nodeName);
    UA_NodeId parentNodeId          = UA_NODEID_NUMERIC(0, UA_NS0ID_OBJECTSFOLDER);
    UA_NodeId parentReferenceNodeId = UA_NODEID_NUMERIC(0, UA_NS0ID_ORGANIZES);
    UA_QualifiedName browseName     = UA_QUALIFIEDNAME_ALLOC(1, nodeName);
    UA_NodeId variableType          = UA_NODEID_NULL;

    UA_Server_addVariableNode(
                server,
                temperatureNodeId,
                parentNodeId,
                parentReferenceNodeId,
                browseName,
                variableType,
                attr,
                NULL,
                NULL);
}

int main(void) {
    signal(SIGINT, stopHandler);
    signal(SIGTERM, stopHandler);

    UA_ServerConfig *config = UA_ServerConfig_new_default();
    UA_Server *server = UA_Server_new(config);

    createTemperatureVariableNode(server);

    UA_StatusCode retval = UA_Server_run(server, &running);
    UA_Server_delete(server);
    UA_ServerConfig_delete(config);
    return (int)retval;
}