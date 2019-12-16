#!/bin/bash

set -euo pipefail

source bash/util/flags.bash
source bash/util/functions.bash

function run_build_and_test() {
    local TITLE="Running build and test ( $( print_condition_for_build_and_test ) )"

    print_job \
    "${TITLE}"

    if $( should_run_build_and_test ${@} ) ;
    then
        print_job \
        "${TITLE}" \
        "done"
    else
        print_job \
        "${TITLE}" \
        "skipped"
    fi
}

run_build_and_test ${@}
