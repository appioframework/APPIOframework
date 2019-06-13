#ifndef _TANKSIMULATION_C_
#define _TANKSIMULATION_C_

#include <time.h>
#include <stdio.h>

double gLiquidLevel                = 90000.0;    //[ml] max 100l = 100 000ml
UA_DateTime gTimeStamp;
const double LEAKAGE_PER_SECOND    = 200.0;
const uint32_t MAX_TANK_LEVEL    = 100000;

void tankInit()
{
    gTimeStamp = UA_DateTime_now();
}

uint32_t getLiquidLevel()
{
    UA_DateTime currentTimeStamp = UA_DateTime_now();
    double timeDifferenceSec =  (double)(currentTimeStamp - gTimeStamp) / UA_DATETIME_SEC;
    gTimeStamp = currentTimeStamp;

	double leakage        = timeDifferenceSec * LEAKAGE_PER_SECOND;

    FILE *file;
    uint32_t pumpStream;
    if(file = fopen("/etc/appio/volumeStreamValue.txt", "r"))
    {
        char sPumpStream[10];
        fgets(sPumpStream, 10, file);
        pumpStream = atoi(sPumpStream);
    	fclose(file);
    }
    else
    {
        pumpStream = 0;
    }
    
    gLiquidLevel += timeDifferenceSec * (pumpStream - LEAKAGE_PER_SECOND);
    gLiquidLevel = gLiquidLevel < 0.0 ? 0.0 : gLiquidLevel > (double)MAX_TANK_LEVEL ? MAX_TANK_LEVEL : gLiquidLevel;

    return (uint32_t)gLiquidLevel;
}

#endif
