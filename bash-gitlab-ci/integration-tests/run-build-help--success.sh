#!/bin/bash

set -euo pipefail

if [ "${1}" = "verbose" ];
then
  oppo build --help
else
  oppo build -h
fi