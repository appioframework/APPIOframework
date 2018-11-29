#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]='oppo build --name "my-app"'
VAR_COMMANDS[1]='oppo build -n     "my-app"'

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir build-opcuaapp--success
  cd    build-opcuaapp--success

  oppo new opcuaapp -n "my-app"
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  ${VAR_COMMAND}

  if [ ! -f "./my-app/build/client-app" ];
  then
    echo "deployable client application file does not exist ..."
    exit 1
  fi

  if [[ ! -x "./my-app/build/client-app" ]]
  then
    echo "deployable client application file is not executable ..."
    exit 1
  fi

  if [ ! -f "./my-app/build/server-app" ];
  then
    echo "deployable server application file does not exist ..."
    exit 1
  fi

  if [[ ! -x "./my-app/build/server-app" ]]
  then
    echo "deployable server application file is not executable ..."
    exit 1
  fi

  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf build-opcuaapp--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done