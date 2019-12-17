#!/bin/bash

set -euo pipefail

source bash/util/flags.bash
source bash/util/functions.bash
source bash/util/localhost.bash

function run_job_help() {
    local CI_JOB_ID="help"
    local CI_JOB_PRIVILEGE="${USER}"
    local CI_JOB_SCRIPT="bash/inject/${CI_JOB_ID}.bash"

    local TITLE="Running 'appio --help' ( $( print_condition_for_help ) )"

    print_job \
    "${TITLE}"

    if $( should_run_help ${@} ) ;
    then
        run_ci_job \
        "${CI_JOB_ID}" \
        "${CI_JOB_PRIVILEGE}" \
        "${CI_JOB_SCRIPT}"

        print_job \
        "${TITLE}" \
        "${DID_FAIL}"
    else
        print_job \
        "${TITLE}" \
        "skipped"
    fi

    exit ${DID_FAIL}
}

run_job_help ${@}
