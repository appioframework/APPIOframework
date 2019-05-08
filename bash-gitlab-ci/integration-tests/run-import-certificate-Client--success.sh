#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="oppo import certificate -n my-app --cert mycert.der --key mykey.der"
VAR_COMMANDS[1]="oppo import certificate -n my-app --cert mycert.pem --key mykey.der"
VAR_COMMANDS[2]="oppo import certificate -n my-app --cert mycert.pem --key mykey.key"
VAR_COMMANDS[3]="oppo import certificate -n my-app --cert mycert.crt --certificate-format pem --key mykey.key"
VAR_COMMANDS[4]="oppo import certificate -n my-app --cert mycert.crt --certificate-format PEM --key mykey.key"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir import-certificate-Client--success
  cd    import-certificate-Client--success

  oppo new opcuaapp -n "my-app" -t "Client" -u "127.0.0.1" -p "4840" --nocert
  rm --force "./oppo.log"

  precondition_oppo_log_file_is_not_existent

  echo "generating manual certificate"
  yes '' | openssl req -new -x509 -out mycert.der -outform der -nodes -keyout mykey.key -days 90
  openssl rsa -outform der -in mykey.key -out mykey.der
  openssl rsa x509 -inform der -outform pem -in mycert.der -out mycert.pem

  ${VAR_COMMAND}

  check_for_exisiting_file_named "./my-app/cert.der" \
                                 "key not imported ..."

  check_for_exisiting_file_named "./my-app/priv.der" \
                                 "certificate not imported ..."
                                 
  openssl  rsa -in ./my-app/priv.der -inform der -noout
  openssl x509 -in ./my-app/cert.der -inform der -noout                               
                                 
  check_for_exisiting_oppo_log_file

  cd ..
  rm -rf import-certificate-Client--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done