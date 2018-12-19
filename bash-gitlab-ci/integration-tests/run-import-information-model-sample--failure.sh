#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo import information-model -n my-a/pp -s"
VAR_COMMANDS[1]="oppo import information-model -n my-app -S"
VAR_COMMANDS[2]="oppo import information-model -n my-app --samples"
VAR_COMMANDS[3]="oppo import information-model -n my-app --Sample"
VAR_COMMANDS[4]="oppo import information-model -n my-app --sampLe"
VAR_COMMANDS[5]="oppo import information-model -n my-a/pp --sample"
VAR_COMMANDS[6]="oppo import information-model -nn my-a/pp --sample"
VAR_COMMANDS[7]="oppo import information-model  my-a/pp --sample"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir import-information-model--failure
  cd    import-information-model--failure

  oppo new opcuaapp -n "my-app"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}
  
  check_for_non_zero_error_code
  
  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf import-information-model--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done