#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo publish --name my-app"
VAR_COMMANDS[1]="oppo publish -n     my-app"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir publish-opcuaapp--success
  cd    publish-opcuaapp--success

  oppo new opcuaapp --name "my-app"
  oppo build        --name "my-app"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_oppo_log_file

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