#!/bin/bash

set -euo pipefail

if [ "${1}" = "verbose" ];
then
  oppo clean --help
else
  oppo clean -h
fi