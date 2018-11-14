#!/bin/bash

set -uo pipefail

VAR_COMMANDS[0]="oppo new sln --name \"my/\-app"\"
VAR_COMMANDS[1]="oppo new sln -n     \"my/\-app"\"
VAR_COMMANDS[4]="oppo new sln --name"
VAR_COMMANDS[5]="oppo new sln -n"
VAR_COMMANDS[2]="oppo new sln --exit"
VAR_COMMANDS[3]="oppo new sln -x"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir new-sln--failure
  cd    new-sln--failure

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
  rm -rf new-sln--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done