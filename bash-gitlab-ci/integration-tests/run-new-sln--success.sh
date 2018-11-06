#!/bin/bash

set -euo pipefail

mkdir sln-new--success
cd    sln-new--success

if [ "${1}" = "verbose" ];
then
  oppo new sln --name "my-solution"
else
  oppo new sln -n "my-solution"
fi

if [ ! -f "my-solution.opposln" ];
then
  echo "oppo solution file does not exist ..."
  exit 1
fi