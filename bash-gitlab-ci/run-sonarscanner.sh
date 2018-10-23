#!/bin/bash

set -euo pipefail

export PATH="$PATH:/root/.dotnet/tools"                \
   && dotnet sonarscanner begin /k:"oppo-terminal" /d:sonar.host.url="https://sonarqube.talsen.team" /d:sonar.cs.opencover.reportsPaths="oppo-terminal.tests/coverage.opencover.xml" \
   && dotnet build oppo-terminal.sln                   \
   && dotnet sonarscanner end