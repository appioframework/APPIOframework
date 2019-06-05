#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio sln add -s testSln -p testProj"
VAR_COMMANDS[1]="appio sln add -s testSln --project testProj"
VAR_COMMANDS[2]="appio sln add --solution testSln -p testProj"
VAR_COMMANDS[3]="appio sln add --solution testSln --project testProj"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir sln-add--success
  cd    sln-add--success

  appio new opcuaapp -n "testProj" -t "ClientServer" -u "127.0.0.1" -p "4840"
  appio new sln -n "testSln"
  rm --force "./appio.log"

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_appio_log_file

  cd ..
  rm -rf sln-add--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done