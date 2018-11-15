#!/bin/bash

set -uo pipefail

VAR_COMMANDS[0]="oppo publish opcuaapp --name \"my/\-app\""
VAR_COMMANDS[1]="oppo publish opcuaapp -n     \"my/\-app\""
VAR_COMMANDS[2]="oppo publish opcuaapp --name"
VAR_COMMANDS[3]="oppo publish opcuaapp -n"
VAR_COMMANDS[4]="oppo publish opcuaapp --exit"
VAR_COMMANDS[5]="oppo publish opcuaapp -x"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir new-opcuaapp--failure
  cd    new-opcuaapp--failure

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
  rm -rf new-opcuaapp--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done