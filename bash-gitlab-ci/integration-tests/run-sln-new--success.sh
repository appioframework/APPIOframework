#!/bin/bash

set -euo pipefail

mkdir sln-new--success
cd    sln-new--success
oppo sln new -n "my-solution"

if [ ! -f "my-solution.opposln" ];
then
  echo "oppo solution file does not exist ..."
  exit 1
fi