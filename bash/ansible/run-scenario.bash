#!/bin/bash

set -euo pipefail

function run_scenario() {
    local MOLECULE_CACHE_DIR=${1}
    local SCENARIO=${2}
    local EXIT_CODE=0

    set +e
    molecule \
    test \
    --scenario-name=${SCENARIO} \
    > ${MOLECULE_CACHE_DIR}/${SCENARIO}.log \
    2>&1
    EXIT_CODE=${?}
    set -e

    echo ${EXIT_CODE} \
    > ${MOLECULE_CACHE_DIR}/${SCENARIO}.exit
}

run_scenario ${@}
