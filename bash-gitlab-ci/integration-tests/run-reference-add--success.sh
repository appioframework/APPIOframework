#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio reference add -c clientName -s serverName"
VAR_COMMANDS[1]="appio reference add -c clientName --server serverName"
VAR_COMMANDS[2]="appio reference add --client clientName -s serverName"
VAR_COMMANDS[3]="appio reference add --client clientName --server serverName"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir reference-add--success
  cd    reference-add--success

  appio new opcuaapp -n "clientName" -t "Client"
  appio new opcuaapp -n "serverName" -t "Server" -u "127.0.0.1" -p "4840"
  rm --force "./appio.log"

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_appio_log_file

  cd ..
  rm -rf reference-add--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done