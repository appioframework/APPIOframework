#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]='oppo build --name "my/\-app"'
VAR_COMMANDS[1]='oppo build -n     "my/\-app"'
VAR_COMMANDS[2]='oppo build --name'
VAR_COMMANDS[3]='oppo build -n'
VAR_COMMANDS[4]='oppo build --exit'
VAR_COMMANDS[5]='oppo build -x'

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir new-opcuaapp--failure
  cd    new-opcuaapp--failure

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  if [ ${?} = 0 ];
  then
    echo "failing command did not result in exit code != 0 ..."
    exit 1
  fi

  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf new-opcuaapp--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done