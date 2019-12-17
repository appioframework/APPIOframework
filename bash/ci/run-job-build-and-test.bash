#!/bin/bash

set -euo pipefail

source bash/util/docker.bash
source bash/util/flags.bash
source bash/util/functions.bash

function run_build_and_test() {
    local CI_JOB_ARTIFACTS=""
    local CI_JOB_ID="build-and-test"
    local CI_JOB_IMAGE="appioframework/dotnet-core:v2.1-sdk"
    local CI_JOB_SCRIPT="bash/inject/${CI_JOB_ID}.bash"

    local TITLE="Running build and test ( $( print_condition_for_build_and_test ) )"

    print_job \
    "${TITLE}"

    if $( should_run_build_and_test ${@} ) ;
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

run_build_and_test ${@}
