#!/bin/bash

set -euo pipefail

mkdir clean-help--success
cd    clean-help--success

if [ "${1}" = "verbose" ];
then
  oppo clean --help
else
  oppo clean -h
fi

if [ ! -f "./oppo.log" ];
then
  echo "no log entry was created ..."
  exit 1
fi

cd ..
rm -rf clean-help--success