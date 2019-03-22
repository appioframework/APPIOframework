#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo sln remove -n solutionName -p projectName"
VAR_COMMANDS[1]="oppo sln remove -s solutionName -n projectName"
VAR_COMMANDS[2]="oppo sln remove -p solutionName -s projectName"
VAR_COMMANDS[3]="oppo sln remove -s solutionName"
VAR_COMMANDS[4]="oppo sln remove -p projectName"


for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir sln-remove--failure
  cd    sln-remove--failure

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code
  
  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf sln-remove--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done