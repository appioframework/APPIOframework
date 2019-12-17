#!/bin/bash

set -euo pipefail

function dotnet_build() {
    dotnet \
    build \
    --configuration Release \
    appio.build.sln 
}

function dotnet_test() {
    dotnet \
    test \
    --configuration Release \
    appio.tests.sln \
    /p:CollectCoverage=true \
    /p:CoverletOutputFormat=opencover
}

function build_and_test() {
    dotnet_test
    dotnet_build
}

build_and_test ${@}
