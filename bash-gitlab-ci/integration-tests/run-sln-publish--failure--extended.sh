#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo sln publish -s solutionName"
VAR_COMMANDS[1]="oppo sln publish --solution solutionName"

echo "Testing failure of package preparation ..."

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir sln-publish--failure--extended
  cd    sln-publish--failure--extended

  oppo new opcuaapp --name "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840"
  oppo new sln --name "testSln"
  oppo sln build -s "testSln"
  rm --force "./testSln.sln"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code
  
  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf sln-publish--failure--extended

  echo "Testing command '${VAR_COMMAND}' ... done"
done