#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo sln remove -n mySln -p myProj"
VAR_COMMANDS[1]="oppo sln remove -s mySln -n myProj"
VAR_COMMANDS[2]="oppo sln remove -p mySln -s myProj"
VAR_COMMANDS[3]="oppo sln remove -s mySln"
VAR_COMMANDS[4]="oppo sln remove -p myProj"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir sln-remove-opcuaapp--failure
  cd    sln-remove-opcuaapp--failure

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code
  
  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf sln-remove-opcuaapp--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done