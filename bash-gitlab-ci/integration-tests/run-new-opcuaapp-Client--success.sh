#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio new opcuaapp -n my-app -t Client"
VAR_COMMANDS[1]="appio new opcuaapp -n my-app --type Client"
VAR_COMMANDS[2]="appio new opcuaapp --name my-app -t Client"
VAR_COMMANDS[3]="appio new opcuaapp --name my-app --type Client"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir new-opcuaapp-Client--success
  cd    new-opcuaapp-Client--success

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_appio_log_file

  check_for_exisiting_file_named "./my-app/my-app.appioproj" \
                                 "appio project file does not exist ..."

  check_for_exisiting_file_named "./my-app/meson.build" \
                                 "meson.build file does not exist ..."

  check_for_exisiting_file_named "./my-app/src/client/main.c" \
                                 "any appio project source file for the client application does not exist ..."
                                 
  check_for_exisiting_file_named "./my-app/certificates/priv.der" \
                                 "private key was not generated ..."
                                 
  check_for_exisiting_file_named "./my-app/certificates/cert.der" \
                                 "certificate was not generated ..."
                                 
  openssl  rsa -in ./my-app/certificates/priv.der -inform der -noout
  openssl x509 -in ./my-app/certificates/cert.der -inform der -noout

  check_for_exisiting_file_named "./my-app/src/client/globalVariables.h" \
                                 "any appio project source file for the client application does not exist ..."

  cd ..
  rm -rf new-opcuaapp-Client--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done