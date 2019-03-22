#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo generate -n my-app -m model.xml"
VAR_COMMANDS[1]="oppo generate -n my-app --model model.xml"
VAR_COMMANDS[2]="oppo generate --name my-app -m model.xml"
VAR_COMMANDS[3]="oppo generate --name my-app --model model.xml"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir generate--failure
  cd    generate--failure

  oppo new opcuaapp -n "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent
  
  ${VAR_COMMAND}
  
  check_for_non_zero_error_code
  
  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf generate--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done