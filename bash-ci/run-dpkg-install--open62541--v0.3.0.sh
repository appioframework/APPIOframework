#!/bin/bash

set -euo pipefail

cd installer
dpkg --install open62541--v0.3.0.deb
