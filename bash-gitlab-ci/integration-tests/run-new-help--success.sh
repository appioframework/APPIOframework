#!/bin/bash

set -euo pipefail

mkdir new-help--success
cd    new-help--success

if [ "${1}" = "verbose" ];
then
  oppo new --help
else
  oppo new -h
fi

if [ ! -f "./oppo.log" ];
then
  echo "no log entry was created ..."
  exit 1
fi

cd ..
rm -rf new-help--success