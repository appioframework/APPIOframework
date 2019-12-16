#!/bin/bash

source bash/util/functions.bash

CI_JOB_WORKDIR="/APPIOframework"

function ci_job_destroy() {
    local CI_JOB_ID=${1}

    local TITLE="${CI_JOB_ID} > Destroy"

    print_entry \
    "${TITLE}"

    if [ $( sudo docker ps --all --quiet --filter name=${CI_JOB_ID} ) ];
    then
        begin_observe_exit_code
        sudo docker kill ${CI_JOB_ID} &> /dev/null
        sudo docker rm ${CI_JOB_ID} > /dev/null
        end_observe_exit_code

        print_entry \
        "${TITLE}" \
        "done"
    else
        print_entry \
        "${TITLE}" \
        "skipped"
    fi
}

function ci_job_create() {
    local CI_JOB_ID=${1}
    local CI_JOB_IMAGE=${2}

    local TITLE="${CI_JOB_ID} > Create"

    print_entry \
    "${TITLE}"

    begin_observe_exit_code
    sudo \
    docker run \
    --detach \
    --name=${CI_JOB_ID} \
    --workdir=${CI_JOB_WORKDIR} \
    ${CI_JOB_IMAGE} \
    sleep infinity \
    > /dev/null
    end_observe_exit_code

    print_entry \
    "${TITLE}" \
    "done"
}

function ci_job_prepare() {
    local CI_JOB_ID=${1}

    local TITLE="${CI_JOB_ID} > Prepare"

    print_entry \
    "${TITLE}"

    begin_observe_exit_code
    sudo \
    docker cp \
    --archive \
    . \
    ${CI_JOB_ID}:${CI_JOB_WORKDIR}
    end_observe_exit_code

    print_entry \
    "${TITLE}" \
    "done"
}

function ci_job_inject() {
    local CI_JOB_ID=${1}
    local CI_JOB_SCRIPT=${2}

    local TITLE="${CI_JOB_ID} > Inject"

    print_entry \
    "${TITLE}"

    begin_observe_exit_code
    sudo \
    docker exec \
    ${CI_JOB_ID} \
    /bin/bash \
    ${CI_JOB_SCRIPT}
    end_observe_exit_code

    print_entry \
    "${TITLE}" \
    "${EXIT_CODE}"
}

function print_ci_job_id() {
    echo "stage--${1}"
}
