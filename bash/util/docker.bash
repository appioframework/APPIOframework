#!/bin/bash

source bash/util/functions.bash

CI_JOB_WORKDIR="/APPIOframework"

function ci_job_destroy() {
    local CI_JOB_ID=${1}

    local TITLE="${CI_JOB_ID} > Destroy"

    print_entry \
    "${TITLE}"

    set +e
    if [ $( sudo docker ps --all --quiet --filter name=${CI_JOB_ID} ) ];
    then
        sudo docker kill ${CI_JOB_ID} &> /dev/null
        sudo docker rm ${CI_JOB_ID} > /dev/null

        print_entry \
        "${TITLE}" \
        "done"
    else
        print_entry \
        "${TITLE}" \
        "skipped"
    fi
    set -e
}

function ci_job_create() {
    local CI_JOB_ID=${1}
    local CI_JOB_IMAGE=${2}

    local TITLE="${CI_JOB_ID} > Create"

    print_entry \
    "${TITLE}"

    sudo \
    docker run \
    --detach \
    --name=${CI_JOB_ID} \
    --workdir=${CI_JOB_WORKDIR} \
    ${CI_JOB_IMAGE} \
    sleep infinity \
    > /dev/null

    print_entry \
    "${TITLE}" \
    "done"
}

function print_ci_job_id() {
    echo "stage--${1}"
}
