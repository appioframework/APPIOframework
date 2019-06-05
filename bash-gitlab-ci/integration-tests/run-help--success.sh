#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio --help"
VAR_COMMANDS[1]="appio -h"
VAR_COMMANDS[2]="appio help"
VAR_COMMANDS[3]="appio ?"
VAR_COMMANDS[4]="appio"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir help--success
  cd    help--success

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_appio_log_file

  cd ..
  rm -rf help--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done