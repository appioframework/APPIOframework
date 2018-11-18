#!/bin/bash

set -euo pipefail

mkdir publish-opcuaapp--success
cd    publish-opcuaapp--success

oppo new opcuaapp --name "my-app"
oppo build -n "my-app"

if [ "${1}" = "verbose" ];
then
  oppo publish --name "my-app"
else
  oppo publish -n "my-app"
fi

if [ ! -f "./my-app/publish/app" ];
then
  echo "published application file is missing ..."
  exit 1
fi

if [[ ! -x "./my-app/publish/app" ]]
then
  echo "published application file is not executable ..."
  exit 1
fi

cd ..
rm -rf publish-opcuaapp--success