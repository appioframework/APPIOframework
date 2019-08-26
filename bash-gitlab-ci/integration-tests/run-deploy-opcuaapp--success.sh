#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

VAR_COMMANDS[0]="appio deploy --name my-app"
VAR_COMMANDS[1]="appio deploy -n     my-app"

for INDEX in "${!VAR_COMMANDS[@]}";
do
  VAR_COMMAND=${VAR_COMMANDS[INDEX]}
  
  echo "Testing command '${VAR_COMMAND}' ..."

  mkdir deploy-opcuaapp--success
  cd    deploy-opcuaapp--success

  appio new opcuaapp --name "my-app" -t "ClientServer" -u "127.0.0.1" -p "4840"
  appio build        --name "my-app"
  appio publish      --name "my-app"
  rm --force "./appio.log"

  precondition_appio_log_file_is_not_existent

  ${VAR_COMMAND}

  check_for_exisiting_appio_log_file

  check_for_exisiting_file_named "./my-app/deploy/appio-opcuaapp.deb" \
                                 "deployable debian installer is missing ..."

  check_for_missing_directory_named "./my-app/deploy/temp" \
                                    "deploy temp directory is still existing ..."

 # dpkg --install ./my-app/deploy/appio-opcuaapp.deb
 sudo apt install ./my-app/deploy/appio-opcuaapp.deb

  check_for_exisiting_file_named "/usr/bin/client-app" \
                                 "installed client application is missing ..."

  check_for_exisiting_file_named "/usr/bin/server-app" \
                                 "installed server application is missing ..."

  #dpkg --purge appio-opcuaapp-installer
  sudo apt-get purge -y appio-opcuaapp-installer
  
  check_for_missing_file_named "/usr/bin/client-app" \
                               "un-installed client application is still existent ..."

  check_for_missing_file_named "/usr/bin/server-app" \
                               "un-installed server application is still existent ..."

  cd ..
  rm -rf deploy-opcuaapp--success

  echo "Testing command '${VAR_COMMAND}' ... done"
done