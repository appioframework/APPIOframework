#!/bin/bash

set -euo pipefail

dotnet test -c Release oppo-terminal.tests.sln /p:CollectCoverage=true /p:CoverletOutputFormat=opencover \
 | grep "Total Line: *"