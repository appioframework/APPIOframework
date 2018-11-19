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

if [ ! -f "./my-app/publish/client-app" ];
then
  echo "published client application file is missing ..."
  exit 1
fi

if [[ ! -x "./my-app/publish/client-app" ]]
then
  echo "published client application file is not executable ..."
  exit 1
fi

if [ ! -f "./my-app/publish/server-app" ];
then
  echo "published server application file is missing ..."
  exit 1
fi

if [[ ! -x "./my-app/publish/server-app" ]]
then
  echo "published server application file is not executable ..."
  exit 1
fi

cd ..
rm -rf publish-opcuaapp--success