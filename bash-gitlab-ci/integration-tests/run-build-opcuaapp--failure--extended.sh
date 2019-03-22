#!/bin/bash

set -uo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo build --name my-app"
VAR_COMMANDS[1]="oppo build -n     my-app"

echo "Testing failure of meson call ..."

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir build-opcuaapp--failure--extended
  cd    build-opcuaapp--failure--extended

  oppo new opcuaapp --name my-app -t "ClientServer" -u "127.0.0.1" -p "4840"
  rm --force "./oppo.log"

  mv "./my-app/meson.build" "./my-app/meson.build.bak"

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code

  check_for_exisiting_oppo_log_file

  cd     ..
  rm -rf build-opcuaapp--failure--extended

  echo "Testing command '${VAR_COMMAND}' ... done"
done

echo "Testing failure of meson call ... done"

# ---

VAR_COMMANDS[0]="oppo build --name my-app"
VAR_COMMANDS[1]="oppo build -n     my-app"

echo "Testing failure of ninja call ..."

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir build-opcuaapp--failure--extended
  cd    build-opcuaapp--failure--extended

  oppo new opcuaapp --name my-app
  rm --force "./oppo.log"

  echo "// removed source code for this test case ..." > "./my-app/src/server/main.c"

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_non_zero_error_code

  check_for_exisiting_oppo_log_file

  cd     ..
  rm -rf build-opcuaapp--failure--extended

  echo "Testing command '${VAR_COMMAND}' ... done"
done

echo "Testing failure of ninja call ... done"