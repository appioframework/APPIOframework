#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio publish --help"
VAR_COMMANDS[1]="appio publish -h"
VAR_COMMANDS[2]="appio publish"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir publish-help--success
  cd    publish-help--success

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_appio_log_file

  cd ..
  rm -rf publish-help--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done