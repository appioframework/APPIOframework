#!/bin/bash

set -euo pipefail

if [ "${1}" = "verbose" ];
then
  oppo new --help
else
  oppo new -h
fi