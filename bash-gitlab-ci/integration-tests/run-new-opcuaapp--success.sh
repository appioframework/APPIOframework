#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo new opcuaapp --name my-app"
VAR_COMMANDS[1]="oppo new opcuaapp -n     my-app"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir new-opcuaapp--success
  cd    new-opcuaapp--success

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_oppo_log_file

  check_for_exisiting_directory_named "./my-app/models" \
                                      "models directory does not exist ..."

  check_for_exisiting_file_named "./my-app/my-app.oppoproj" \
                                 "oppo project file does not exist ..."

  check_for_exisiting_file_named "./my-app/meson.build" \
                                 "meson.build file does not exist ..."

  check_for_exisiting_file_named "./my-app/src/client/main.c" \
                                 "any oppo project source file for the client application does not exist ..."

  check_for_exisiting_file_named "./my-app/src/server/main.c" \
                                 "any oppo project source file for the server application does not exist ..."

  check_for_exisiting_file_named "./my-app/src/server/models.c" \
                                 "any oppo project source file for the server application does not exist ..."

  check_for_exisiting_file_named "./my-app/src/server/nodeSetFunctioncs.c" \
                                 "any oppo project source file for the server application does not exist ..."

  cd ..
  rm -rf new-opcuaapp--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done