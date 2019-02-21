#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo sln --help"
VAR_COMMANDS[1]="oppo sln -h"
VAR_COMMANDS[2]="oppo sln"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir sln-help--success
  cd    sln-help--success

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf sln-help--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done