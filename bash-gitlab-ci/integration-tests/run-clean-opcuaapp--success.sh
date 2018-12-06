#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo clean --name my-app"
VAR_COMMANDS[1]="oppo clean -n     my-app"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir clean-opcuaapp--success
  cd    clean-opcuaapp--success

  oppo new opcuaapp -n "my-app"
  oppo build        -n "my-app"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_directory_named "./my-app/build" \
                                      "build directory was not removed ..."

  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf clean-opcuaapp--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done