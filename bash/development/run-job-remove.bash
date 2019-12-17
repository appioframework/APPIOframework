#!/bin/bash

set -euo pipefail

source bash/util/flags.bash
source bash/util/functions.bash
source bash/util/localhost.bash

function run_job_remove() {
    local CI_JOB_ID="remove"
    local CI_JOB_PRIVILEGE="root"
    local CI_JOB_SCRIPT="bash/inject/${CI_JOB_ID}.bash"

    local TITLE="Running remove ( $( print_condition_for_remove ) )"

    print_job \
    "${TITLE}"

    if $( should_run_remove ${@} ) ;
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

run_job_remove ${@}
