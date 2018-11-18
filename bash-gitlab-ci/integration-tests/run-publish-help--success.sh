#!/bin/bash

set -euo pipefail

mkdir publish-help--success
cd    publish-help--success

if [ "${1}" = "verbose" ];
then
  oppo publish --help
else
  oppo publish -h
fi

if [ ! -f "./oppo.log" ];
then
  echo "no log entry was created ..."
  exit 1
fi

cd ..
rm -rf publish-help--success