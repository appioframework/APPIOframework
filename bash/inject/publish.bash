#!/bin/bash

set -euo pipefail

function publish() {
    local CACHE_DIR="cache"

    mkdir \
    --parents \
    ${CACHE_DIR}

    dotnet \
    publish \
    --configuration Release \
    --output ${CACHE_DIR}/publish \
    src/appio-terminal/appio-terminal.csproj
}

publish
