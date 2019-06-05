#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio import information-model -n my-app -s"
VAR_COMMANDS[1]="appio import information-model -n my-app --sample"
VAR_COMMANDS[2]="appio import information-model --name my-app -s"
VAR_COMMANDS[3]="appio import information-model --name my-app --sample"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir import-information-model--success
  cd    import-information-model--success

  appio new opcuaapp -n "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840" --nocert
  rm --force "./appio.log"

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_file_named "./my-app/models/DiNodeset.xml" \
                                 "information-model import failed ..."

  check_for_exisiting_file_named "./my-app/models/DiTypes.bsd" \
                                 "information-model import failed ..."

  check_for_exisiting_appio_log_file

  cd ..
  rm -rf import-information-model--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done