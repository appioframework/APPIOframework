#!/bin/bash

set -euo pipefail

dotnet test -c Release oppo.tests.sln /p:CollectCoverage=true /p:CoverletOutputFormat=opencover 
 