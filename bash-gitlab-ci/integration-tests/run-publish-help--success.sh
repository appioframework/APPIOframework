#!/bin/bash

set -euo pipefail

if [ "${1}" = "verbose" ];
then
  oppo publish --help
else
  oppo publish -h
fi