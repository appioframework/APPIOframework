#!/bin/bash

set -uo pipefail

VAR_COMMANDS[0]="appio publish --name \"my/\-app\""
VAR_COMMANDS[1]="appio publish -n     \"my/\-app\""
VAR_COMMANDS[2]="appio publish --name"
VAR_COMMANDS[3]="appio publish -n"
VAR_COMMANDS[4]="appio publish --exit"
VAR_COMMANDS[5]="appio publish -x"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir publish-opcuaapp--failure
  cd    publish-opcuaapp--failure

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code
  
  check_for_exisiting_appio_log_file

  cd ..
  rm -rf publish-opcuaapp--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done