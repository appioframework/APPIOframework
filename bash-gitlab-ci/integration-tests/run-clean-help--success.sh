#!/bin/bash

set -euo pipefail

if [ "${1}" = "verbose" ];
then
  oppo clean --help
else
  oppo clean -h
fi

if [ ! -f "oppo.log" ];
then
  echo "no log entry was created ..."
  exit 1
fi