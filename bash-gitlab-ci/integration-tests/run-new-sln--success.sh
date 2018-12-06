#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo new sln --name my-app"
VAR_COMMANDS[1]="oppo new sln -n     my-app"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir new-sln--success
  cd    new-sln--success

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_oppo_log_file

  check_for_exisiting_file_named "./my-solution.opposln" \
                                 "oppo solution file does not exist ..."

  cd ..
  rm -rf new-sln--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done