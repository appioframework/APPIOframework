#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio new opcuaapp -n my-app -t ClientServer -u 127.0.0.1 -p 4840"
VAR_COMMANDS[1]="appio new opcuaapp -n my-app -t ClientServer -u 127.0.0.1 --port 4840"
VAR_COMMANDS[2]="appio new opcuaapp -n my-app -t ClientServer --url 127.0.0.1 -p 4840"
VAR_COMMANDS[3]="appio new opcuaapp -n my-app -t ClientServer --url 127.0.0.1 --port 4840"
VAR_COMMANDS[4]="appio new opcuaapp -n my-app --type ClientServer -u 127.0.0.1 -p 4840"
VAR_COMMANDS[5]="appio new opcuaapp -n my-app --type ClientServer -u 127.0.0.1 --port 4840"
VAR_COMMANDS[6]="appio new opcuaapp -n my-app --type ClientServer --url 127.0.0.1 -p 4840"
VAR_COMMANDS[7]="appio new opcuaapp -n my-app --type ClientServer --url 127.0.0.1 --port 4840"
VAR_COMMANDS[8]="appio new opcuaapp --name my-app -t ClientServer -u 127.0.0.1 -p 4840"
VAR_COMMANDS[9]="appio new opcuaapp --name my-app -t ClientServer -u 127.0.0.1 --port 4840"
VAR_COMMANDS[10]="appio new opcuaapp --name my-app -t ClientServer --url 127.0.0.1 -p 4840"
VAR_COMMANDS[11]="appio new opcuaapp --name my-app -t ClientServer --url 127.0.0.1 --port 4840"
VAR_COMMANDS[12]="appio new opcuaapp --name my-app --type ClientServer -u 127.0.0.1 -p 4840"
VAR_COMMANDS[13]="appio new opcuaapp --name my-app --type ClientServer -u 127.0.0.1 --port 4840"
VAR_COMMANDS[14]="appio new opcuaapp --name my-app --type ClientServer --url 127.0.0.1 -p 4840"
VAR_COMMANDS[15]="appio new opcuaapp --name my-app --type ClientServer --url 127.0.0.1 --port 4840"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir new-opcuaapp-ClientServer--success
  cd    new-opcuaapp-ClientServer--success

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_appio_log_file

  check_for_exisiting_directory_named "./my-app/models" \
                                      "models directory does not exist ..."

  check_for_exisiting_file_named "./my-app/my-app.appioproj" \
                                 "appio project file does not exist ..."

  check_for_exisiting_file_named "./my-app/meson.build" \
                                 "meson.build file does not exist ..."

  check_for_exisiting_file_named "./my-app/src/client/main.c" \
                                 "any appio project source file for the client application does not exist ..."

  check_for_exisiting_file_named "./my-app/src/client/globalVariables.h" \
                                 "any appio project source file for the client application does not exist ..."

  check_for_exisiting_file_named "./my-app/src/server/main.c" \
                                 "any appio project source file for the server application does not exist ..."

  check_for_exisiting_file_named "./my-app/src/server/meson.build" \
                                 "any appio project source file for the server application does not exist ..."

  check_for_exisiting_file_named "./my-app/src/server/loadInformationModels.c" \
                                 "any appio project source file for the server application does not exist ..."

  check_for_exisiting_file_named "./my-app/src/server/constants.h" \
                                 "any appio project source file for the server application does not exist ..."

  check_for_exisiting_file_named "./my-app/src/server/mainCallbacks.c" \
                                 "any appio project source file for the server application does not exist ..."

  check_for_exisiting_file_named "./my-app/certificates/client_priv.der" \
                                 "client private key was not generated ..."
                                 
  check_for_exisiting_file_named "./my-app/certificates/client_cert.der" \
                                 "client certificate was not generated ..."

  check_for_exisiting_file_named "./my-app/certificates/server_priv.der" \
                                 "server private key was not generated ..."
                                 
  check_for_exisiting_file_named "./my-app/certificates/server_cert.der" \
                                 "server certificate was not generated ..."

  openssl  rsa -in ./my-app/certificates/server_priv.der -inform der -noout
  openssl x509 -in ./my-app/certificates/server_cert.der -inform der -noout
  openssl  rsa -in ./my-app/certificates/client_priv.der -inform der -noout
  openssl x509 -in ./my-app/certificates/client_cert.der -inform der -noout
  
  cd ..
  rm -rf new-opcuaapp-ClientServer--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done