#!/bin/bash

set -euo pipefail

cd installer/open62541--v1.0.0
chmod -R 0775 DEBIAN
cd ..
dpkg --build open62541--v1.0.0
