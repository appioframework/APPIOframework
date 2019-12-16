#!/bin/bash

set -euo pipefail

function run_dotnet_build() {
    dotnet \
    build \
    --configuration Release \
    appio.build.sln 
}

function run_dotnet_test() {
    dotnet \
    test \
    --configuration Release \
    appio.tests.sln \
    /p:CollectCoverage=true \
    /p:CoverletOutputFormat=opencover
}

run_dotnet_test
run_dotnet_build
