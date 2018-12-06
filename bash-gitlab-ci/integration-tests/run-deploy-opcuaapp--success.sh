#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo deploy --name my-app"
VAR_COMMANDS[1]="oppo deploy -n     my-app"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir deploy-opcuaapp--success
  cd    deploy-opcuaapp--success

  oppo new opcuaapp --name "my-app"
  oppo build        --name "my-app"
  oppo publish      --name "my-app"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_oppo_log_file

  check_for_exisiting_file_named "./my-app/deploy/oppo-opcuaapp.deb" \
                                 "deployable debian installer is missing ..."

  check_for_missing_directory_named "./my-app/deploy/temp" \
                                    "deploy temp directory is still existing ..."

  dpkg --install ./my-app/deploy/oppo-opcuaapp.deb

  check_for_exisiting_file_named "/usr/bin/client-app" \
                                 "installed client application is missing ..."

  check_for_exisiting_file_named "/usr/bin/server-app" \
                                 "installed server application is missing ..."

  dpkg --purge oppo-opcuaapp-installer

  check_for_missing_file_named "/usr/bin/client-app" \
                               "un-installed client application is still existent ..."

  check_for_missing_file_named "/usr/bin/server-app" \
                               "un-installed server application is still existent ..."

  cd ..
  rm -rf deploy-opcuaapp--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done