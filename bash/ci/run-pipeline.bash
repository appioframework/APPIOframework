#!/bin/bash

set -euo pipefail

source bash/util/functions.bash

function run_pipeline() {
    print_title \
    "Running CI pipeline"

    print_title \
    "Running CI pipeline" \
    "done"
}

run_pipeline ${@}
