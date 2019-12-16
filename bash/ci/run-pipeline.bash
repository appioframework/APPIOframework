#!/bin/bash

set -euo pipefail

source bash/util/functions.bash

function run_pipeline_stages_via_docker() {
    local TITLE="Running stages via docker"

    print_stage \
    "${TITLE}"

    begin_observe_exit_code
    /bin/bash \
    bash/ci/run-build-and-test.bash ${@}
    end_observe_exit_code

    print_stage \
    "${TITLE}" \
    "${EXIT_CODE}"
}

function run_pipeline() {
    local TITLE="Running CI pipeline"

    print_title \
    "${TITLE}"

    run_pipeline_stages_via_docker \
    ${@}

    print_title \
    "${TITLE}" \
    "${EXIT_CODE}"

    exit ${EXIT_CODE}
}

run_pipeline ${@}
