#!/bin/bash

set -euo pipefail

source bash/config/appio-terminal.bash
source bash/config/cache-dir.bash
source bash/config/open62541.bash

source bash/util/functions.bash

function install() {
    dpkg \
    --install \
    ${CACHE_DIR}/installer/${OPEN62541_VERSION}.deb

    dpkg \
    --install \
    ${CACHE_DIR}/installer/${APPIO_TERMINAL_VERSION}.deb
}

install ${@}
