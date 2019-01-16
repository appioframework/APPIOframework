#!/bin/bash

set -euo pipefail

cd installer/oppo-terminal
chmod -R 0775 DEBIAN
mkdir -p usr/bin
cp ../../publish/* usr/bin
rm -f usr/bin/*.pdb
cd ..
dpkg --build oppo-terminal