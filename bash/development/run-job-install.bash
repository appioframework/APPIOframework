#!/bin/bash

set -euo pipefail

source bash/util/flags.bash
source bash/util/functions.bash
source bash/util/localhost.bash

function run_job_install() {
    local CI_JOB_ID="install"
    local CI_JOB_PRIVILEDGE="root"
    local CI_JOB_SCRIPT="bash/inject/${CI_JOB_ID}.bash"

    local TITLE="Running install ( $( print_condition_for_install ) )"

    print_job \
    "${TITLE}"

    if $( should_run_install ${@} ) ;
    then
        run_ci_job \
        "${CI_JOB_ID}" \
        "${CI_JOB_PRIVILEDGE}" \
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

run_job_install ${@}
