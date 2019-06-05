#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio deploy --name my-app"
VAR_COMMANDS[1]="appio deploy -n     my-app"

echo "Testing failure of package preparation ..."

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir build-opcuaapp--failure--extended
  cd    build-opcuaapp--failure--extended

  appio new opcuaapp --name "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840"
  appio build        --name "my-app"
  appio publish      --name "my-app"
  rm --force "./appio.log"
  
  mv "./my-app/publish/client-app" "./my-app/publish/client-app.bak"
  mv "./my-app/publish/server-app" "./my-app/publish/server-app.bak"

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code

  check_for_exisiting_appio_log_file

  cd     ..
  rm -rf build-opcuaapp--failure--extended

  echo "Testing command '${VAR_COMMAND}' ... done"
done

echo "Testing failure of preparation ... done"