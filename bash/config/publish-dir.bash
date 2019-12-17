#!/bin/bash

source bash/config/cache-dir.bash

PUBLISH_DIR=${CACHE_DIR}/publish
PUBLISH_DIR_ABS=$( realpath ${PUBLISH_DIR} )
