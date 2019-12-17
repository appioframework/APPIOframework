#!/bin/bash

set -euo pipefail

function check_dotnet_sdk() {
    which \
    dotnet

    dotnet \
    --version

    dotnet \
    --help
}

check_dotnet_sdk ${@}
