#!/bin/bash

set -euo pipefail

source bash/util/docker.bash
source bash/util/flags.bash
source bash/util/functions.bash

function run_build_and_test() {
    local CI_JOB_IMAGE="appioframework/dotnet-core:v2.1-sdk"

    local TITLE="Running build and test ( $( print_condition_for_build_and_test ) )"

    print_job \
    "${TITLE}"

    if $( should_run_build_and_test ${@} ) ;
    then
        local CI_JOB_ID=$( print_ci_job_id "build-and-test" )

        ci_job_destroy \
        "${CI_JOB_ID}"

        ci_job_create \
        "${CI_JOB_ID}" \
        "${CI_JOB_IMAGE}"

        ci_job_prepare \
        "${CI_JOB_ID}"

        ci_job_inject \
        "${CI_JOB_ID}" \
        bash/inject/build-and-test.bash

        ci_job_collect \
        "${CI_JOB_ID}" \
        ""

        ci_job_destroy \
        "${CI_JOB_ID}"

        print_job \
        "${TITLE}" \
        "${EXIT_CODE}"
    else
        print_job \
        "${TITLE}" \
        "skipped"
    fi

    exit ${EXIT_CODE}
}

run_build_and_test ${@}
