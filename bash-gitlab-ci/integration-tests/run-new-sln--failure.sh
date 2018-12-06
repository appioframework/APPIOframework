#!/bin/bash

set -uo pipefail

VAR_COMMANDS[0]="oppo new sln --name \"my/\-app\""
VAR_COMMANDS[1]="oppo new sln -n     \"my/\-app\""
VAR_COMMANDS[2]="oppo new sln --name"
VAR_COMMANDS[3]="oppo new sln -n"
VAR_COMMANDS[4]="oppo new sln --exit"
VAR_COMMANDS[5]="oppo new sln -x"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir new-sln--failure
  cd    new-sln--failure

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code
  
  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf new-sln--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done