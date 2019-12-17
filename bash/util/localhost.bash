#!/bin/bash

function ci_job_inject() {
    local CI_JOB_ID=${1}
    local CI_JOB_PRIVILEDGE=${2}
    local CI_JOB_SCRIPT=${3}

    local TITLE="${CI_JOB_ID} > Inject"

    print_entry \
    "${TITLE}"

    begin_observe_exit_code
    /bin/bash \
    bash/util/elevate.bash \
    ${CI_JOB_PRIVILEDGE} \
    /bin/bash \
    ${CI_JOB_SCRIPT}
    end_observe_exit_code

    print_entry \
    "${TITLE}" \
    "${EXIT_CODE}"
}

function run_ci_job() {
    local CI_JOB_ID=${1}
    local CI_JOB_PRIVILEDGE=${2}
    local CI_JOB_SCRIPT=${3}

    ci_job_inject \
    "${CI_JOB_ID}" \
    "${CI_JOB_PRIVILEDGE}" \
    "${CI_JOB_SCRIPT}"
}
