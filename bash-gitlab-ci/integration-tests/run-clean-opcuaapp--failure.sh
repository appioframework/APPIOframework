#!/bin/bash

set -uo pipefail

VAR_COMMANDS[0]="oppo clean --name \"my/\-app\""
VAR_COMMANDS[1]="oppo clean -n     \"my/\-app\""
VAR_COMMANDS[2]="oppo clean --name"
VAR_COMMANDS[3]="oppo clean -n"
VAR_COMMANDS[4]="oppo clean --exit"
VAR_COMMANDS[5]="oppo clean -x"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir clean-opcuaapp--failure
  cd    clean-opcuaapp--failure

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
  rm -rf clean-opcuaapp--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done