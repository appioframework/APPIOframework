#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo import certificate -p my-app --certificate mycert.der --key mykey.der"
VAR_COMMANDS[1]="oppo import certificate -p my-app --certificate mycert.pem --key mykey.der"
VAR_COMMANDS[2]="oppo import certificate -p my-app --certificate mycert.pem --key mykey.key"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir import-certificate-Client--success
  cd    import-certificate-Client--success

  oppo new opcuaapp -n "my-app" -t "Client" -u "127.0.0.1" -p "4840" --nocert
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  generate_crypto_all_formats

  ${VAR_COMMAND}

  check_for_exisiting_file_named "./my-app/certificates/cert.der" \
                                 "key not imported ..."

  check_for_exisiting_file_named "./my-app/certificates/priv.der" \
                                 "certificate not imported ..."
                                 
  openssl  rsa -in ./my-app/certificates/priv.der -inform der -noout
  openssl x509 -in ./my-app/certificates/cert.der -inform der -noout                               
                                 
  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf import-certificate-Client--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done