#!/bin/bash

set -euo pipefail

source bash-gitlab-ci/util-integration-tests.sh

mkdir build-help--success
cd    build-help--success

precondition_oppo_log_file_is_not_existent

if [ "${1}" = "verbose" ];
then
  oppo build --help
else
  oppo build -h
fi

check_for_exisiting_oppo_log_file

cd ..
rm -rf build-help--success