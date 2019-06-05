#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio import information-model -n my-app -p model.txt"
VAR_COMMANDS[1]="appio import information-model -n my-app --path model.xml2"
VAR_COMMANDS[2]="appio import information-model -n my-app --Path model.xml2"
VAR_COMMANDS[3]="appio import information-model -n my-app -P model.xml2"
VAR_COMMANDS[4]="appio import information-model -n my-app -p notExistingModel.xml"
VAR_COMMANDS[5]="appio import information-model -nn my-app -p notExistingModel.xml"
VAR_COMMANDS[6]="appio import information-model my-app -p notExistingModel.xml"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir import-information-model--failure
  cd    import-information-model--failure

  appio new opcuaapp -n "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840" --nocert
  rm --force "./appio.log"

  precondition_appio_log_file_is_not_existent

  echo "creating dummy model.xml"
  touch "model.xml"
  
  ${VAR_COMMAND}
  
  check_for_non_zero_error_code
  
  check_for_exisiting_appio_log_file

  cd ..
  rm -rf import-information-model--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done