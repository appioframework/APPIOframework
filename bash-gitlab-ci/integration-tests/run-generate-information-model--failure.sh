#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo generate information-model -n my-app -m model.txt"
VAR_COMMANDS[1]="oppo generate information-model -n my-app --model model.xml2"
VAR_COMMANDS[2]="oppo generate information-model -n my-app --Model model.xml2"
VAR_COMMANDS[3]="oppo generate information-model -n my-app -M model.xml2"
VAR_COMMANDS[4]="oppo generate information-model -n my-app -m notExistingModel.xml"
VAR_COMMANDS[5]="oppo generate information-model -nn my-app -m notExistingModel.xml"
VAR_COMMANDS[6]="oppo generate information-model my-app -m notExistingModel.xml"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir generate-information-model--failure
  cd    generate-information-model--failure

  oppo new opcuaapp -n "my-app" -t "Client"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  cd my-app/models
  touch model.txt
  touch model.xml2
  cd ../..
  
  ${VAR_COMMAND}
  
  check_for_non_zero_error_code
  
  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf generate-information-model--failure

  echo "Testing command '${VAR_COMMAND}' ... done"
done