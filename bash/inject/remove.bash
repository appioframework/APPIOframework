#!/bin/bash

set -euo pipefail

source bash/config/appio-terminal.bash
source bash/config/open62541.bash

source bash/util/functions.bash

function run_dpkg_remove() {
    dpkg \
    --remove \
    ${APPIO_TERMINAL_VERSION}

    dpkg \
    --remove \
    ${OPEN62541_VERSION}
}

run_dpkg_remove ${@}
