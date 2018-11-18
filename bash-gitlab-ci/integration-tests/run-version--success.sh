#!/bin/bash

set -euo pipefail

mkdir version--success
cd    version--success

oppo version

if [ ! -f "./oppo.log" ];
then
  echo "no log entry was created ..."
  exit 1
fi

cd ..
rm -rf version--success