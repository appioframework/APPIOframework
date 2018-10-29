#!/bin/bash

set -euo pipefail

dotnet test -c Release oppo-terminal.tests.sln /p:CollectCoverage=true /p:CoverletOutputFormat=opencover \
 | grep "Total Line: *"

ls -Al src/oppo-terminal.tests/bin/Release/netcoreapp2.1