#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio new sln --name my-solution"
VAR_COMMANDS[1]="appio new sln -n     my-solution"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir new-sln--success
  cd    new-sln--success

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_appio_log_file

  check_for_exisiting_file_named "./my-solution.appiosln" \
                                 "appio solution file does not exist ..."

  cd ..
  rm -rf new-sln--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done