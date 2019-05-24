#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo generate information-model -n my-app -m minimumCompilableModel.xml"
VAR_COMMANDS[1]="oppo generate information-model -n my-app --model minimumCompilableModel.xml"
VAR_COMMANDS[2]="oppo generate information-model --name my-app -m minimumCompilableModel.xml"
VAR_COMMANDS[3]="oppo generate information-model --name my-app --model minimumCompilableModel.xml"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir generate-information-model--success
  cd    generate-information-model--success

  oppo new opcuaapp -n "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  cd my-app/models
  echo "<UANodeSet xmlns=\"http://opcfoundation.org/UA/2011/03/UANodeSet.xsd\"><Models><Model ModelUri=\"test\" Version=\"1.01\" PublicationDate=\"2012-12-31T00:00:00Z\"></Model></Models></UANodeSet>" > minimumCompilableModel.xml
  cd ../..

  ${VAR_COMMAND}

  oppo build -n my-app

  check_for_exisiting_file_named "./my-app/src/server/information-models/minimumCompilableModel.c" \
                                 "information-model .c file does not exist ..."

  check_for_exisiting_file_named "./my-app/src/server/information-models/minimumCompilableModel.h" \
                                 "information-model .h file does not exist ..."

  check_for_exisiting_file_named "./my-app/build/server-app" \
								 "server-app file does not exist ..."

  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf generate-information-model--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done