#!/bin/bash

set -uo pipefail

VAR_COMMANDS[0]="oppo deploy --name \"my/\-app\""
VAR_COMMANDS[1]="oppo deploy -n     \"my/\-app\""
VAR_COMMANDS[2]="oppo deploy --name"
VAR_COMMANDS[3]="oppo deploy -n"
VAR_COMMANDS[4]="oppo deploy --exit"
VAR_COMMANDS[5]="oppo deploy -x"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir deploy--failure
  cd    deploy--failure

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
  rm -rf deploy--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done