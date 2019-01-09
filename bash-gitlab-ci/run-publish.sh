#!/bin/bash

set -euo pipefail

cd src/oppo-terminal
dotnet publish -c Release -o ../../publish
