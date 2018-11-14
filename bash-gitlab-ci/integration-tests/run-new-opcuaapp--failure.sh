#!/bin/bash

set -uo pipefail

mkdir new-opcuaapp--failure
cd    new-opcuaapp--failure

oppo new opcuaapp --name "my/\-app"

if [ ${?} = 0 ];
then
  echo "failing command did not result in exit code != 0 ..."
  exit 1
fi

if [ ! -f "oppo.log" ];
then
  echo "no log entry was created ..."
  exit 1
fi

cd ..
rm -rf new-opcuaapp--failure

# mkdir new-opcuaapp--failure
# cd    new-opcuaapp--failure

# oppo new opcuaapp -n "my/\-app"

# if [ ! -f "oppo.log" ];
# then
#   echo "no log entry was created ..."
#   exit 1
# fi

# cd ..
# rm -rf new-opcuaapp--failure