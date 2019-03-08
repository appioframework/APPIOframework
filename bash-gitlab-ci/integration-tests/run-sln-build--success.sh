#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo sln build -s testSln"
VAR_COMMANDS[1]="oppo sln build --solution testSln"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir sln-build--success
  cd    sln-build--success

  oppo new opcuaapp -n "testProj" -t "ClientServer" -u "127.0.0.1" -p "4840"
  oppo new sln -n "testSln"
  oppo sln add -s "testSln" -p "testProj"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_directory_named "./testProj/build/" \
                                 "meson output directory does not exist ..."

  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf sln-build--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done