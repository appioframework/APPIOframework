#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo import information-model my-app -p model.xml"
VAR_COMMANDS[1]="oppo import information-model my-app --path model.xml"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir import-information-model--success
  cd    import-information-model--success

  oppo new opcuaapp -n "my-app"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  echo "creating dummy model.xml"
  touch "model.xml"
  ${VAR_COMMAND}

  if [ ! -f "./my-app/models/model.xml" ];
  then
    echo "information-model import failed! ..."
    exit 1
  fi
  
  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf import-information-model--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done