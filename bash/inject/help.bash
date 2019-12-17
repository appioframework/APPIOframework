#!/bin/bash

set -euo pipefail

function help() {
    appio \
    --help
}

help ${@}
