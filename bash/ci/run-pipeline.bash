#!/bin/bash

set -euo pipefail

source bash/util/functions.bash

function run_pipeline_jobs_via_ansible() {
    local TITLE="Running jobs via ansible"

    print_stage \
    "${TITLE}"

    begin_observe_exit_code
    /bin/bash \
    bash/ci/run-job-system-test.bash ${@}
    end_observe_exit_code

    print_stage \
    "${TITLE}" \
    "${DID_FAIL}"
}

function run_pipeline_jobs_via_docker() {
    local TITLE="Running jobs via docker"

    print_stage \
    "${TITLE}"

    begin_observe_exit_code
    /bin/bash \
    bash/ci/run-job-build-and-test.bash ${@}
    end_observe_exit_code

    begin_observe_exit_code
    /bin/bash \
    bash/ci/run-job-publish.bash ${@}
    end_observe_exit_code

    begin_observe_exit_code
    /bin/bash \
    bash/ci/run-job-build-installer.bash ${@}
    end_observe_exit_code

    print_stage \
    "${TITLE}" \
    "${DID_FAIL}"
}

function run_pipeline() {
    local STAMP_START=$( date +%s )

    local TITLE="Running CI pipeline"

    print_title \
    "${TITLE}"

    run_pipeline_jobs_via_docker \
    ${@}

    run_pipeline_jobs_via_ansible \
    ${@}

    print_title \
    "${TITLE}" \
    "${EXIT_CODE}"

    local STAMP_END=$( date +%s )
    local DURATION=$(( STAMP_END - STAMP_START ))

    print_title \
    "Pipeline ran for ${DURATION} seconds"

    exit ${DID_FAIL}
}

run_pipeline ${@}
