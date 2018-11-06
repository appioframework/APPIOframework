#!/bin/bash

set -euo pipefail

mkdir build-opcuaapp--success
cd    build-opcuaapp--success

oppo new opcuaapp -n "my-app"

if [ "${1}" = "verbose" ];
then
  oppo build --name "my-app"
else
  oppo build -n "my-app"
fi

if [ ! -f "my-app/build/app" ];
then
  echo "deployable application file does not exist ..."
  exit 1
fi

if [[ ! -x "my-app/build/app" ]]
then
  echo "deployable application file is not executable ..."
  exit 1
fi

cd ..
rm -rf build-opcuaapp--success