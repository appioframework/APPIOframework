#!/bin/bash

set -euo pipefail

dotnet test -c Release appio.tests.sln /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
