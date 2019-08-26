#!/bin/bash

set -euo pipefail

cd installer
#dpkg --install open62541--v1.0.0.deb
sudo apt install ./open62541--v1.0.0.deb