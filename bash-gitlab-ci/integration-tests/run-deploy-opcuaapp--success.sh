#!/bin/bash

set -euo pipefail

mkdir deploy-opcuaapp--success
cd    deploy-opcuaapp--success

oppo new opcuaapp --name "my-app"
oppo build   -n "my-app"
oppo publish -n "my-app"

if [ "${1}" = "verbose" ];
then
  oppo deploy --name "my-app"
else
  oppo deploy -n "my-app"
fi

if [ ! -f "./my-app/deploy/oppo-opcuaapp.deb" ];
then
  echo "deployable debian installer is missing ..."
  exit 1
fi

if [ -d "./my-app/deploy/temp" ];
then
  echo "deploy temp directory is still existing ..."
  exit 1
fi

dpkg --install ./my-app/deploy/oppo-opcuaapp.deb

if [ ! -f "/usr/bin/client-app" ];
then
  echo "deployed client application is missing ..."
  exit 1
fi

if [ ! -f "/usr/bin/server-app" ];
then
  echo "deployed server application is missing ..."
  exit 1
fi

dpkg --purge oppo-opcuaapp-installer

cd ..
rm -rf deploy-opcuaapp--success