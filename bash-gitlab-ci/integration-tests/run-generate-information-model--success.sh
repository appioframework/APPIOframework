#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio generate information-model -n my-app"
VAR_COMMANDS[1]="appio generate information-model --name my-app"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir generate-information-model--success
  cd    generate-information-model--success

  appio new opcuaapp -n "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840" --nocert
  echo "<UANodeSet xmlns=\"http://opcfoundation.org/UA/2011/03/UANodeSet.xsd\"><Models><Model ModelUri=\"test\" Version=\"1.01\" PublicationDate=\"2012-12-31T00:00:00Z\"></Model></Models></UANodeSet>" > minimumCompilableModel.xml
  appio import information-model -n "my-app" -p "minimumCompilableModel.xml"

  rm --force "./appio.log"

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  appio build -n my-app

  check_for_exisiting_file_named "./my-app/src/server/information-models/minimumCompilableModel.c" \
                                 "information-model .c file does not exist ..."

  check_for_exisiting_file_named "./my-app/src/server/information-models/minimumCompilableModel.h" \
                                 "information-model .h file does not exist ..."

  check_for_exisiting_appio_log_file

  cd ..
  rm -rf generate-information-model--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done