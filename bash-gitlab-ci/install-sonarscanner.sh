#!/bin/bash
set -euo pipefail
cat << \EOF >> ~/.bash_profile
# Add .NET Core SDK tools
export PATH="$PATH:/root/.dotnet/tools"
EOF

dotnet tool install --global dotnet-sonarscanner