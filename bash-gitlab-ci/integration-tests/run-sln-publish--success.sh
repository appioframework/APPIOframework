#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo sln publish -s testSln"
VAR_COMMANDS[1]="oppo sln publish --solution testSln"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir sln-publish--success
  cd    sln-publish--success

  oppo new opcuaapp -n "testProj" -t "ClientServer" -u "127.0.0.1" -p "4840"
  oppo new sln -n "testSln"
  oppo sln add -s "testSln" -p "testProj"
  oppo sln build -s "testSln"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_directory_named "./testProj/publish/" \
                                 "meson output directory does not exist ..."

  check_for_exisiting_file_named "./testProj/publish/client-app" \
                                 "published client application file is missing ..."

  check_for_exisiting_file_named "./testProj/publish/server-app" \
                                 "published server application file is missing ..."

  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf sln-publish--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done