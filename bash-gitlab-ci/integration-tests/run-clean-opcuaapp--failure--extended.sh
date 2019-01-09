#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo clean --name my-app"
VAR_COMMANDS[1]="oppo clean -n     my-app"

echo "Testing failure of cleaning non existing project ..."

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir clean-opcuaapp--failure--extended
  cd    clean-opcuaapp--failure--extended

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code

  check_for_exisiting_oppo_log_file

  cd     ..
  rm -rf clean-opcuaapp--failure--extended

  echo "Testing command '${VAR_COMMAND}' ... done"
done

echo "Testing failure of cleaning non existing project ... done"