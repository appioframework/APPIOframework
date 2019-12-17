#!/bin/bash

set -euo pipefail

source bash/config/appio-terminal.bash
source bash/config/open62541.bash

source bash/util/docker.bash
source bash/util/flags.bash
source bash/util/functions.bash

function run_publish() {
    local CI_JOB_ARTIFACTS="cache/installer/${OPEN62541_VERSION}.deb cache/installer/${APPIO_TERMINAL_VERSION}.deb"
    local CI_JOB_ID="build-installer"
    local CI_JOB_IMAGE="ubuntu:bionic"
    local CI_JOB_SCRIPT="bash/inject/${CI_JOB_ID}.bash"

    local TITLE="Running installer build ( $( print_condition_for_build_installer ) )"

    print_job \
    "${TITLE}"

    if $( should_run_build_installer ${@} ) ;
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

run_publish ${@}
