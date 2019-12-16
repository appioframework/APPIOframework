#!/bin/bash

source bash/util/functions.bash

CI_JOB_WORKDIR="/APPIOframework"

function ci_job_destroy() {
    local CI_JOB_ID=${1}

    print_entry \
    "Destroying ${CI_JOB_ID}"

    set +e
    if [ $( sudo docker ps --all --quiet --filter name=${CI_JOB_ID} ) ];
    then
        sudo docker kill ${CI_JOB_ID} &> /dev/null
        sudo docker rm ${CI_JOB_ID} > /dev/null

        print_entry \
        "Destroying ${CI_JOB_ID}" \
        "done"
    else
        print_entry \
        "Destroying ${CI_JOB_ID}" \
        "skipped"
    fi
    set -e
}

function print_ci_job_id() {
    echo "stage--${1}"
}
