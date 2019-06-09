#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio import certificate -n my-app --certificate mycert.der --key mykey.der --server"
VAR_COMMANDS[1]="appio import certificate -n my-app --certificate mycert.pem --key mykey.der --server"
VAR_COMMANDS[2]="appio import certificate --server -n my-app --certificate mycert.pem --key mykey.key"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir import-certificate-ClientServer--success
  cd    import-certificate-ClientServer--success

  appio new opcuaapp -n "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840" --nocert
  rm --force "./appio.log"

  precondition_appio_log_file_is_not_existent

  generate_crypto_all_formats

  ${VAR_COMMAND}

  check_for_exisiting_file_named "./my-app/certificates/server_cert.der" \
                                 "key not imported ..."

  check_for_exisiting_file_named "./my-app/certificates/server_priv.der" \
                                 "certificate not imported ..."

  openssl  rsa -in ./my-app/certificates/server_priv.der -inform der -noout
  openssl x509 -in ./my-app/certificates/server_cert.der -inform der -noout
                                 
  check_for_exisiting_appio_log_file

  cd ..
  rm -rf import-certificate-ClientServer--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done