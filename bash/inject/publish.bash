#!/bin/bash

set -euo pipefail

source bash/config/publish-dir.bash

function publish() {
    mkdir \
    --parents \
    ${PUBLISH_DIR}

    dotnet \
    publish \
    --configuration Release \
    --output ${PUBLISH_DIR_ABS} \
    src/appio-terminal/appio-terminal.csproj
}

publish
