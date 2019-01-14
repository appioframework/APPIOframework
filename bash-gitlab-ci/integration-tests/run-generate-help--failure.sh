#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo generate -help"
VAR_COMMANDS[1]="oppo generate -H"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir generate-help--failure
  cd    generate-help--failure

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf generate-help--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done