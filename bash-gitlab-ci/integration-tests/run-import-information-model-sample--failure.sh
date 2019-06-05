#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio import information-model -s"
VAR_COMMANDS[1]="appio import information-model --sample"
VAR_COMMANDS[2]="appio import information-model -n my-a/pp -s"
VAR_COMMANDS[3]="appio import information-model -n my-app -S"
VAR_COMMANDS[4]="appio import information-model -n my-app --samples"
VAR_COMMANDS[5]="appio import information-model -n my-app --Sample"
VAR_COMMANDS[6]="appio import information-model -n my-app --sampLe"
VAR_COMMANDS[7]="appio import information-model -n my-a/pp --sample"
VAR_COMMANDS[8]="appio import information-model -nn my-a/pp --sample"
VAR_COMMANDS[9]="appio import information-model my-a/pp --sample"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir import-information-model--failure
  cd    import-information-model--failure

  appio new opcuaapp -n "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840"
  rm --force "./appio.log"

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}
  
  check_for_non_zero_error_code
  
  check_for_exisiting_appio_log_file

  cd ..
  rm -rf import-information-model--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done