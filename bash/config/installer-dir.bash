#!/bin/bash

source bash/config/cache-dir.bash
source bash/config/project-dir.bash

INSTALLER_DIR=${CACHE_DIR}/installer
INSTALLER_DIR_ABS=$( realpath ${INSTALLER_DIR} )

INSTALLER_SRC_DIR=${PROJECT_DIR}/installer
INSTALLER_SRC_DIR_ABS=$( realpath ${INSTALLER_DIR} )
