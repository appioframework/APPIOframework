#include "open62541.h"
#include <time.h>
#include <stdio.h>

#define MIN_LIQUID_LEVEL 85000
#define MAX_LIQUID_LEVEL 95000

void writeInfoToConsoleAndLogFile(UA_UInt32 liquidLevel, bool isFillingTank)
{
    time_t now = time(0);
    struct tm *currentTime = localtime(&now);
    
    // write to console
    // timestamp
    printf("\n[%d-%d-%d %d:%d:%d]", currentTime->tm_year + 1900, currentTime->tm_mon, currentTime->tm_mday, currentTime->tm_hour, currentTime->tm_min, currentTime->tm_sec);
    
    // liquid level
    printf("\nThe value of liquid level is: %d\n", liquidLevel);
    
    // pump method call
    if(isFillingTank) printf("StartPump()");
    else printf("StopPump()");
    printf(" on mvpSmartPump server called!\n");
    
    //write to file
    FILE *file;
    file = fopen("liquidLevelLog.txt", "a");

    // timestamp
    fprintf(file, "\n[%d-%d-%d %d:%d:%d]", currentTime->tm_year + 1900, currentTime->tm_mon, currentTime->tm_mday, currentTime->tm_hour, currentTime->tm_min, currentTime->tm_sec);
    
    // liquid level
    fprintf(file, "\nThe value of liquid level is: %d\n", liquidLevel);
    
    // pump method call
    if(isFillingTank) fprintf(file, "StartPump()");
    else fprintf(file, "StopPump()");
    fprintf(file, " on mvpSmartPump server called!\n");
    fclose(file);
}

void manageTank(UA_Client *mvpSmartPumpClient, UA_Client *mvpSmartLiterSensorClient, UA_Boolean *running)
{
    bool isFillingTank = false;
    while(running)
        {
        // read current liquid level
        UA_Variant value;
        UA_Variant_init(&value);
        UA_NodeId NodeId         = UA_NODEID_NUMERIC(3, 1001);
        UA_StatusCode readStatus     = UA_Client_readValueAttribute(mvpSmartLiterSensorClient, NodeId, &value);

      if(readStatus != UA_STATUSCODE_GOOD)
      {
           printf("bad connection to node '%s'!\n", NodeId);
           break;
       }
       else
       {
           UA_Int32 liquidLevel = *(UA_UInt32*)value.data;
           // check if liquid is below the limit
           if(liquidLevel < MIN_LIQUID_LEVEL & !isFillingTank)
           {
               UA_StatusCode callmvpSmartPump_StartPumpMethod  =  UA_Client_call(mvpSmartPumpClient, UA_NODEID_NUMERIC(3, 2000), UA_NODEID_NUMERIC(3, 3000), NULL, NULL, NULL, NULL);
               if(callmvpSmartPump_StartPumpMethod != UA_STATUSCODE_GOOD)
               {
                   printf("could not call StartPump() on mvpSmartPump server!\n");
                   break;
               }
               isFillingTank = true;
               writeInfoToConsoleAndLogFile(liquidLevel, isFillingTank);
           }

           // check if liquid is over the limit
           else if(liquidLevel > MAX_LIQUID_LEVEL & isFillingTank)
           {
               UA_StatusCode callmvpSmartPump_StopPumpMethod  =  UA_Client_call(mvpSmartPumpClient, UA_NODEID_NUMERIC(3, 2000), UA_NODEID_NUMERIC(3, 3001), NULL, NULL, NULL, NULL);
               if(callmvpSmartPump_StopPumpMethod != UA_STATUSCODE_GOOD)
               {
                   printf("could not call StopPump() on mvpSmartPump server!\n");
                   break;
                }
                isFillingTank = false;
                writeInfoToConsoleAndLogFile(liquidLevel, isFillingTank);
            }
        }
    }
}
