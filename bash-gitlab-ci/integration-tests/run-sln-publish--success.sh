#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio sln publish -s testSln"
VAR_COMMANDS[1]="appio sln publish --solution testSln"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir sln-publish--success
  cd    sln-publish--success

  appio new opcuaapp -n "testProj" -t "ClientServer" -u "127.0.0.1" -p "4840"
  appio new sln -n "testSln"
  appio sln add -s "testSln" -p "testProj"
  appio sln build -s "testSln"
  rm --force "./appio.log"

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_directory_named "./testProj/publish/" \
                                 "meson output directory does not exist ..."

  check_for_exisiting_file_named "./testProj/publish/client-app" \
                                 "published client application file is missing ..."

  check_for_exisiting_file_named "./testProj/publish/server-app" \
                                 "published server application file is missing ..."

  check_for_exisiting_appio_log_file

  cd ..
  rm -rf sln-publish--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done