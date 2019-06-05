#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio generate information-model -N my-app"
VAR_COMMANDS[1]="appio generate information-model --n my-app"
VAR_COMMANDS[2]="appio generate information-model --Name my-app"
VAR_COMMANDS[3]="appio generate information-model -name my-app"
VAR_COMMANDS[4]="appio generate information-model -n my-app -m model.xml"
VAR_COMMANDS[5]="appio generate information-model my-app"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir generate-information-model--failure
  cd    generate-information-model--failure

  appio new opcuaapp -n "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840" --nocert
  echo "<UANodeSet xmlns=\"http://opcfoundation.org/UA/2011/03/UANodeSet.xsd\"><Models><Model ModelUri=\"test\" Version=\"1.01\" PublicationDate=\"2012-12-31T00:00:00Z\"></Model></Models></UANodeSet>" > minimumCompilableModel.xml
  appio import information-model -n "my-app" -p "minimumCompilableModel.xml"
  
  rm --force "./appio.log"

  precondition_appio_log_file_is_not_existent
  
  ${VAR_COMMAND}
  
  check_for_non_zero_error_code
  
  check_for_exisiting_appio_log_file

  cd ..
  rm -rf generate-information-model--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done