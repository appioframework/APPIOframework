#!/bin/bash

set -euo pipefail

mkdir sln-new--success
cd    sln-new--success

if [ "${1}" = "verbose" ];
then
  oppo sln new --name "my-solution"
else
  oppo sln new -n "my-solution"
fi

if [ ! -f "my-solution.opposln" ];
then
  echo "oppo solution file does not exist ..."
  exit 1
fi