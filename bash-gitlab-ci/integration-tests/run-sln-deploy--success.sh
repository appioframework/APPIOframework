#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo sln deploy -s testSln"
VAR_COMMANDS[1]="oppo sln deploy --solution testSln"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir sln-deploy--success
  cd    sln-deploy--success

  oppo new opcuaapp -n "testProj" -t "ClientServer" -u "127.0.0.1" -p "4840"
  oppo new sln -n "testSln"
  oppo sln add -s "testSln" -p "testProj"
  oppo sln build -s "testSln"
  oppo sln publish -s "testSln"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_oppo_log_file

  check_for_exisiting_file_named "./testProj/deploy/oppo-opcuaapp.deb" \
                                 "deployable debian installer is missing ..."

  check_for_missing_directory_named "./testProj/deploy/temp" \
                                    "deploy temp directory is still existing ..."

  dpkg --install ./testProj/deploy/oppo-opcuaapp.deb

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
  rm -rf sln-deploy--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done