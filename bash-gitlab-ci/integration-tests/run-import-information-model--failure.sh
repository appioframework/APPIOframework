#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo import information-model my-app -p model.txt"
VAR_COMMANDS[1]="oppo import information-model my-app --path model.xml2"
VAR_COMMANDS[2]="oppo import information-model my-ap//p --path model.xml2"
VAR_COMMANDS[3]="oppo import information-model my-app --Path model.xml2"
VAR_COMMANDS[4]="oppo import information-model my-app -P model.xml2"
VAR_COMMANDS[5]="oppo import information-model my-app -p notExistingModel.xml"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir import-information-model--failure
  cd    import-information-model--failure

  oppo new opcuaapp -n "my-app"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  echo "creating dummy model.xml"
  touch "model.xml"
  
  ${VAR_COMMAND}
  
  if [ ${?} = 0 ];
  then
    echo "import command didn't failed as expected..."
    exit 1
  fi
  
  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf import-information-model--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done