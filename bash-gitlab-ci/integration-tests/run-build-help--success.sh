#!/bin/bash

set -euo pipefail

mkdir build-help--success
cd    build-help--success

if [ "${1}" = "verbose" ];
then
  oppo build --help
else
  oppo build -h
fi

if [ ! -f "./oppo.log" ];
then
  echo "no log entry was created ..."
  exit 1
fi

cd ..
rm -rf build-help--success