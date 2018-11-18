#!/bin/bash

set -uo pipefail

VAR_COMMANDS[0]="oppo publish --name \"my/\-app\""
VAR_COMMANDS[1]="oppo publish -n     \"my/\-app\""
VAR_COMMANDS[2]="oppo publish --name"
VAR_COMMANDS[3]="oppo publish -n"
VAR_COMMANDS[4]="oppo publish --exit"
VAR_COMMANDS[5]="oppo publish -x"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir publish-opcuaapp--failure
  cd    publish-opcuaapp--failure

  ${VAR_COMMAND}

  if [ ${?} = 0 ];
  then
    echo "failing command did not result in exit code != 0 ..."
    exit 1
  fi

  if [ ! -f "./oppo.log" ];
  then
    echo "no log entry was created ..."
    exit 1
  fi

  cd ..
  rm -rf publish-opcuaapp--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done