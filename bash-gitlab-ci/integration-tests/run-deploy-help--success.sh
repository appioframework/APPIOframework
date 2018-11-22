#!/bin/bash

set -euo pipefail

mkdir deploy-help--success
cd    deploy-help--success

if [ "${1}" = "verbose" ];
then
  oppo deploy --help
else
  oppo deploy -h
fi

if [ ! -f "./oppo.log" ];
then
  echo "no log entry was created ..."
  exit 1
fi

cd ..
rm -rf deploy-help--success