#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo reference remove -c clientName -s serverName"
VAR_COMMANDS[1]="oppo reference remove -c clientName --server serverName"
VAR_COMMANDS[2]="oppo reference remove --client clientName -s serverName"
VAR_COMMANDS[3]="oppo reference remove --client clientName --server serverName"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir reference-remove--success
  cd    reference-remove--success

  oppo new opcuaapp -n "clientName" -t "Client"
  oppo new opcuaapp -n "serverName" -t "Server" -u "127.0.0.1" -p "4840"
  oppo reference add -c "clientName" -s "serverName"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf reference-remove--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done