#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo sln build -n solutionName"
VAR_COMMANDS[1]="oppo sln build --name solutionName"
VAR_COMMANDS[2]="oppo sln build -s"
VAR_COMMANDS[3]="oppo sln build --solution"


for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir sln-build--failure
  cd    sln-build--failure

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code
  
  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf sln-build--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done