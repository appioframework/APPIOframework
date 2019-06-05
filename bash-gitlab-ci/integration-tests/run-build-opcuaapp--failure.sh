#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio build --name my-app-5263452364"
VAR_COMMANDS[1]="appio build -n     my-app-5263452364"
VAR_COMMANDS[2]="appio build --name my-a\/pp"
VAR_COMMANDS[3]="appio build -n     my-a\/pp"
VAR_COMMANDS[4]="appio build --name"
VAR_COMMANDS[5]="appio build -n"
VAR_COMMANDS[6]="appio build --exit"
VAR_COMMANDS[7]="appio build -x"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir build-opcuaapp--failure
  cd    build-opcuaapp--failure

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code
  
  check_for_exisiting_appio_log_file

  cd ..
  rm -rf build-opcuaapp--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done