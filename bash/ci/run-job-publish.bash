#!/bin/bash

set -euo pipefail

source bash/util/docker.bash
source bash/util/flags.bash
source bash/util/functions.bash

function run_job_publish() {
    local CI_JOB_ARTIFACTS="cache/publish"
    local CI_JOB_ID="publish"
    local CI_JOB_IMAGE="appioframework/dotnet-core:v2.1-sdk"
    local CI_JOB_SCRIPT="bash/inject/${CI_JOB_ID}.bash"

    local TITLE="Running publish ( $( print_condition_for_publish ) )"

    print_job \
    "${TITLE}"

    if $( should_run_publish ${@} ) ;
    then
        run_ci_job \
        "${CI_JOB_ARTIFACTS}" \
        "${CI_JOB_ID}" \
        "${CI_JOB_IMAGE}" \
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

run_job_publish ${@}
