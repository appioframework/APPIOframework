#!/bin/bash

set -euo pipefail

source bash/util/functions.bash

function run_pipeline_jobs_via_docker() {
    local TITLE="Running jobs via docker"

    print_stage \
    "${TITLE}"

    begin_observe_exit_code
    /bin/bash \
    bash/ci/run-build-and-test.bash ${@}
    end_observe_exit_code

    begin_observe_exit_code
    /bin/bash \
    bash/ci/run-publish.bash ${@}
    end_observe_exit_code

    begin_observe_exit_code
    /bin/bash \
    bash/ci/run-build-installer.bash ${@}
    end_observe_exit_code

    print_stage \
    "${TITLE}" \
    "${DID_FAIL}"
}

function run_pipeline() {
    local TITLE="Running CI pipeline"

    print_title \
    "${TITLE}"

    run_pipeline_jobs_via_docker \
    ${@}

    print_title \
    "${TITLE}" \
    "${EXIT_CODE}"

    exit ${DID_FAIL}
}

run_pipeline ${@}
