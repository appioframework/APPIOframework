#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio reference add -s clientName -c serverName"
VAR_COMMANDS[1]="appio reference add -n clientName -s serverName"
VAR_COMMANDS[2]="appio reference add -c clientName -n serverName"
VAR_COMMANDS[3]="appio reference add -c clientName"
VAR_COMMANDS[4]="appio reference add -s serverName"


for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir reference-add--failure
  cd    reference-add--failure

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code
  
  check_for_exisiting_appio_log_file

  cd ..
  rm -rf reference-add--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done