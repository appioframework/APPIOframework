#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo import information-model -n my-app -p model.xml -t types.bsd"
VAR_COMMANDS[1]="oppo import information-model -n my-app -p model.xml --types types.bsd"
VAR_COMMANDS[2]="oppo import information-model -n my-app --path model.xml -t types.bsd"
VAR_COMMANDS[3]="oppo import information-model -n my-app --path model.xml --types types.bsd"
VAR_COMMANDS[4]="oppo import information-model --name my-app -p model.xml -t types.bsd"
VAR_COMMANDS[5]="oppo import information-model --name my-app -p model.xml --types types.bsd"
VAR_COMMANDS[6]="oppo import information-model --name my-app --path model.xml -t types.bsd"
VAR_COMMANDS[7]="oppo import information-model --name my-app --path model.xml --types types.bsd"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir import-information-model--success--extended
  cd    import-information-model--success--extended

  oppo new opcuaapp -n "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  echo "creating dummy model.xml and types.bsd"
  echo "<?xml version=\"1.0\" encoding=\"utf-8\"?><UANodeSet xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" LastModified=\"2012-12-31T00:00:00Z\" xmlns=\"http://opcfoundation.org/UA/2011/03/UANodeSet.xsd\"><NamespaceUris><Uri>sample_namespace</Uri></NamespaceUris><Models><Model ModelUri=\"sample_namespace\" Version=\"1.01\" PublicationDate=\"2012-12-31T00:00:00Z\" /></Models></UANodeSet>" > model.xml
  touch types.bsd

  ${VAR_COMMAND}

  check_for_exisiting_file_named "./my-app/models/model.xml" \
                                 "information-model import failed ..."

  check_for_exisiting_file_named "./my-app/models/types.xml" \
                                 "information-model import failed ..."

  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf import-information-model--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done