#!/bin/bash

set -euo pipefail

source bash/util/functions.bash

function run_pipeline_stages_via_docker() {
    local TITLE="Running stages via docker"

    print_stage \
    "${TITLE}"

    print_stage \
    "${TITLE}" \
    "done"
}

function run_pipeline() {
    local TITLE="Running CI pipeline"

    print_title \
    "${TITLE}"

    run_pipeline_stages_via_docker \
    ${@}

    print_title \
    "${TITLE}" \
    "done"
}

run_pipeline ${@}
