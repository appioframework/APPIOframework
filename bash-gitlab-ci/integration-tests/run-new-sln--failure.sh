#!/bin/bash

set -uo pipefail

VAR_COMMANDS[0]="appio new sln --name \"my/\-app\""
VAR_COMMANDS[1]="appio new sln -n     \"my/\-app\""
VAR_COMMANDS[2]="appio new sln --name"
VAR_COMMANDS[3]="appio new sln -n"
VAR_COMMANDS[4]="appio new sln --exit"
VAR_COMMANDS[5]="appio new sln -x"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir new-sln--failure
  cd    new-sln--failure

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code
  
  check_for_exisiting_appio_log_file

  cd ..
  rm -rf new-sln--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done