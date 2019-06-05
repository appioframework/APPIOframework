#!/bin/bash

set -uo pipefail

VAR_COMMANDS[0]="appio new opcuaapp --name \"my/\-app\""
VAR_COMMANDS[1]="appio new opcuaapp -n     \"my/\-app\""
VAR_COMMANDS[2]="appio new opcuaapp --name"
VAR_COMMANDS[3]="appio new opcuaapp -n"
VAR_COMMANDS[4]="appio new opcuaapp --exit"
VAR_COMMANDS[5]="appio new opcuaapp -x"
VAR_COMMANDS[6]="appio new opcuaapp -n my-app -type Client"
VAR_COMMANDS[7]="appio new opcuaapp -n my-app --t Server"
VAR_COMMANDS[8]="appio new opcuaapp -p my-app -t ClientServer"
VAR_COMMANDS[9]="appio new opcuaapp -n my-app -t"
VAR_COMMANDS[10]="appio new opcuaapp -n my-app --type"
VAR_COMMANDS[11]="appio new opcuaapp -n my-app"
VAR_COMMANDS[12]="appio new opcuaapp --name my-app"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir new-opcuaapp--failure
  cd    new-opcuaapp--failure

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code
  
  check_for_exisiting_appio_log_file

  cd ..
  rm -rf new-opcuaapp--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done