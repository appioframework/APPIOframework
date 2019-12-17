#!/bin/bash

set -euo pipefail

source bash/config/installer-dir.bash

source bash/config/appio-terminal.bash
source bash/config/cache-dir.bash
source bash/config/open62541.bash

source bash/util/functions.bash

function install() {
    dpkg \
    --install \
    ${INSTALLER_DIR}/${OPEN62541_VERSION}.deb

    dpkg \
    --install \
    ${INSTALLER_DIR}/${APPIO_TERMINAL_VERSION}.deb
}

install ${@}
