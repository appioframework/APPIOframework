#!/bin/bash

set -euo pipefail

mkdir help--success
cd    help--success

oppo help

if [ ! -f "./oppo.log" ];
then
  echo "no log entry was created ..."
  exit 1
fi

cd ..
rm -rf help--success