#!/bin/bash

set -euo pipefail

source bash/util/flags.bash
source bash/util/functions.bash
source bash/util/localhost.bash

function run_job_molecule() {
    local CI_JOB_ID="molecule"
    local CI_JOB_PRIVILEGE="${USER}"
    local CI_JOB_SCRIPT="bash/inject/${CI_JOB_ID}.bash"

    local TITLE="Running molecule test --all ( $( print_condition_for_molecule ) )"

    print_job \
    "${TITLE}"

    if $( should_run_molecule ${@} ) ;
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

run_job_molecule ${@}
