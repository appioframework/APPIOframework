#!/bin/bash

set -euo pipefail

mkdir new-opcuaapp--success
cd    new-opcuaapp--success

if [ "${1}" = "verbose" ];
then
  oppo new opcuaapp --name "my-app"
else
  oppo new opcuaapp -n "my-app"
fi

if [ ! -f "./my-app/my-app.oppoproj" ] && [ ! -f "./my-app/meson.build" ];
then
  echo "oppo project / meson.build file does not exist ..."
  exit 1
fi

if [ ! -f "./my-app/src/main.c" ] && [ ! -f "./my-app/src/open62541.c" ] && [ ! -f "./my-app/src/open62541.h" ];
then
  echo "any oppo project source file does not exist ..."
  exit 1
fi

if [ ! -f "./oppo.log" ];
then
  echo "no log entry was created ..."
  exit 1
fi

cd ..
rm -rf new-opcuaapp--success