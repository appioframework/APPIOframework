#!/bin/bash

set -euo pipefail

export PATH="$PATH:/root/.dotnet/tools"                \
   && dotnet tool install --global dotnet-sonarscanner \
   && dotnet sonarscanner begin /k:"oppo-terminal" /d:sonar.host.url="https://sonarqube.talsen.team" /d:sonar.cs.opencover.reportsPaths="oppo-terminal.tests\coverage.opencover.xml" \
   && dotnet build oppo-terminal.sln                   \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && echo "---" \
   && ls -Al /root/.dotnet/tools/.store/dotnet-sonarscanner/4.4.2/dotnet-sonarscanner/4.4.2/tools/netcoreapp2.1/any/sonar-scanner-3.2.0.1227/bin/sonar-scanner \
   && dotnet sonarscanner end