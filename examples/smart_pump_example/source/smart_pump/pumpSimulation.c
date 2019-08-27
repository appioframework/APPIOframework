#ifndef _PUMP_SIMULATION_C_
#define _PUMP_SIMULATION_C_

#include <stdio.h>

uint32_t gVolumeStream = 0;

void updateFile()
{
    FILE *file;
    file = fopen("/etc/appio/volumeStreamValue.txt", "w");
    fprintf(file, "%u", gVolumeStream);
    fclose(file);
}

void setVolumeStream(uint32_t volume)
{
    if (volume > 800)
    {
        gVolumeStream = 800;
    }
    else
    {
        gVolumeStream = volume;
    }
    updateFile();
}

void startPump()
{
    setVolumeStream(800);
}

void stopPump()
{
    setVolumeStream(0);
}

#endif
