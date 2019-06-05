#!/bin/bash

set -euo pipefail

cd src/appio-terminal
dotnet publish -c Release -o ../../publish
