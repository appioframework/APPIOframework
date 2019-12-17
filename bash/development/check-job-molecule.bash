#!/bin/bash

set -euo pipefail

source bash/util/flags.bash
source bash/util/functions.bash
source bash/util/localhost.bash

function run_job_help() {
    local CI_JOB_ID="check-molecule"
    local CI_JOB_PRIVILEDGE="${USER}"
    local CI_JOB_SCRIPT="bash/inject/${CI_JOB_ID}.bash"

    local TITLE="Checking dependency 'molecule'"

    print_job \
    "${TITLE}"

    run_ci_job \
    "${CI_JOB_ID}" \
    "${CI_JOB_PRIVILEDGE}" \
    "${CI_JOB_SCRIPT}"

    print_job \
    "${TITLE}" \
    "${DID_FAIL}"

    exit ${DID_FAIL}
}

run_job_help ${@}
