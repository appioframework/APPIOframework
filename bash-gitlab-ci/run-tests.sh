#!/bin/bash

set -euo pipefail

dotnet test -c Release src/oppo-objectmodel.tests/oppo-objectmodel.tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover 
dotnet test -c Release src/oppo-terminal.tests/oppo-terminal.tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover 