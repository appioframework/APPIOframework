#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio clean --name my-app"
VAR_COMMANDS[1]="appio clean -n     my-app"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir clean-opcuaapp--success
  cd    clean-opcuaapp--success

  appio new opcuaapp -n "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840"
  appio build        -n "my-app"
  rm --force "./appio.log"

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_missing_directory_named "./my-app/build" \
                                    "build directory was not removed ..."

  check_for_exisiting_appio_log_file

  cd ..
  rm -rf clean-opcuaapp--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done