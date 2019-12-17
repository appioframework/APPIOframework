#!/bin/bash

set -euo pipefail

source bash/config/cache-dir.bash

function publish() {
    mkdir \
    --parents \
    ${CACHE_DIR}

    dotnet \
    publish \
    --configuration Release \
    --output ${CACHE_DIR_ABS}/publish \
    src/appio-terminal/appio-terminal.csproj
}

publish
