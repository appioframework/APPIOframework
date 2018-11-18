#!/bin/bash

set -euo pipefail

mkdir clean-opcuaapp--success
cd    clean-opcuaapp--success

oppo new opcuaapp -n "my-app"
oppo build -n "my-app"
rm --force "./oppo.log"

if [ "${1}" = "verbose" ];
then
  oppo clean --name "my-app"
else
  oppo clean -n "my-app"
fi

if [ -d "./my-app/build" ];
then
  echo "build directory was not removed ..."
  exit 1
fi

if [ ! -f "./oppo.log" ];
then
  echo "no log entry was created ..."
  exit 1
fi

cd ..
rm -rf clean-opcuaapp--success