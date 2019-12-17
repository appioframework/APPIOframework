#!/bin/bash

set -euo pipefail

source bash/config/cache-dir.bash

function help() {
    mkdir \
    --parents \
    ${CACHE_DIR}

    cd \
    ${CACHE_DIR}

    appio \
    --help
}

help ${@}
