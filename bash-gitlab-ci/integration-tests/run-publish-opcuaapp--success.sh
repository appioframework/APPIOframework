#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio publish --name my-app"
VAR_COMMANDS[1]="appio publish -n     my-app"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir publish-opcuaapp--success
  cd    publish-opcuaapp--success

  appio new opcuaapp --name "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840"
  appio build        --name "my-app"
  rm --force "./appio.log"

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_appio_log_file

  check_for_exisiting_file_named "./my-app/publish/client-app" \
                                 "published client application file is missing ..."

  check_for_executable_file "./my-app/publish/client-app" \
                            "published client application file is not executable ..."

  check_for_exisiting_file_named "./my-app/publish/server-app" \
                                 "published server application file is missing ..."

  check_for_executable_file "./my-app/publish/server-app" \
                            "published server application file is not executable ..."

  cd ..
  rm -rf publish-opcuaapp--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done