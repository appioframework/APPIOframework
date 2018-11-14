#!/bin/bash

set -uo pipefail

VAR_COMMANDS[0]="oppo build --name \"my/\-app\""
VAR_COMMANDS[1]="oppo build -n     \"my/\-app\""
VAR_COMMANDS[2]="oppo build --name"
VAR_COMMANDS[3]="oppo build -n"
VAR_COMMANDS[4]="oppo build --exit"
VAR_COMMANDS[5]="oppo build -x"
VAR_COMMANDS[0]="oppo build --name \"my-app\""
VAR_COMMANDS[1]="oppo build -n     \"my-app\""

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir build-opcuaapp--failure
  cd    build-opcuaapp--failure

  oppo new opcuaapp --name "my-app"
  rm --force "oppo.log"
  rm --force "my-app/meson.build"

  ${VAR_COMMAND}

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
  rm -rf build-opcuaapp--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done