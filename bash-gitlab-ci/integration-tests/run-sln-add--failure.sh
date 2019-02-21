#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo sln add -n solutionName -p projectName"
VAR_COMMANDS[0]="oppo sln add -s solutionName -n projectName"
VAR_COMMANDS[0]="oppo sln add -p solutionName -s projectName"
VAR_COMMANDS[0]="oppo sln add -s solutionName"
VAR_COMMANDS[0]="oppo sln add -p projectName"


for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir sln-add--failure
  cd    sln-add--failure

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code
  
  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf sln-add--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done