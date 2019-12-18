#!/bin/bash

set -euo pipefail

source bash/config/cache-dir.bash
source bash/config/project-dir.bash

source bash/util/functions.bash

function instantiate_scenarios() {
    local MOLECULE_CACHE_DIR=${1}
    local MOLECULE_SCENARIOS_DIR=molecule
    local PID=0

    for SCENARIO in $( ls -A ${MOLECULE_SCENARIOS_DIR} | sort --general-numeric-sort )
    do
        MOLECULE_SCENARIOS+=(${SCENARIO})

        /bin/bash ${PROJECT_DIR_ABS}/bash/ansible/run-scenario.bash \
        ${MOLECULE_CACHE_DIR} \
        ${SCENARIO} & \
        PID=${!}

        echo ${PID} \
        > ${MOLECULE_CACHE_DIR}/${SCENARIO}.pid

        print_entry_step \
        "Instantiate scenario '${SCENARIO}' as process '${PID}'"

        sleep \
        10
    done
}

function find_cached_files() {
    local MOLECULE_CACHE_DIR=${1}
    local EXTENSION=${2}

    find ${MOLECULE_CACHE_DIR} -type f -name *.${EXTENSION} -printf "%p\n" | sort --general-numeric-sort
}

function inspect_scenarios() {
    local MOLECULE_CACHE_DIR=${1}
    set +u
    local SHOW_LOG_ON_ERROR_FLAG=${2}
    set -u

    print_entry_step \
    "Inspecting molecule scenarios"

    for PID_FILE in $( find_cached_files ${MOLECULE_CACHE_DIR} pid )
    do
        local SCENARIO=$( basename ${PID_FILE} )
        local SCENARIO=${SCENARIO%.*}
        local PID=$( cat ${PID_FILE} )

        if kill -0 ${PID} > /dev/null 2>&1 ;
        then
            print_entry_step_sub \
            "Scenario '${SCENARIO}' is running"
        else
            local SCENARIO_EXIT=$( cat ${MOLECULE_CACHE_DIR}/${SCENARIO}.exit )

            if [ "${SHOW_LOG_ON_ERROR_FLAG}" = "" ];
            then
                print_entry_step_sub \
                "Scenario '${SCENARIO}' exited with ${SCENARIO_EXIT}"
            elif [[ "${SHOW_LOG_ON_ERROR_FLAG}" = "--show-log-on-error" ]] && [[ ${SCENARIO_EXIT} != 0 ]];
            then
                print_entry_step_sub \
                "Scenario '${SCENARIO}' exited with ${SCENARIO_EXIT}"
                cat ${MOLECULE_CACHE_DIR}/${SCENARIO}.log
                echo ""
            fi
        fi
    done
}

function fail_if_any_scenario_failed() {
    local MOLECULE_CACHE_DIR=${1}

    local HAS_ANY_TEST_FAILED=0

    for EXIT_FILE in $( find_cached_files ${MOLECULE_CACHE_DIR} exit )
    do
        local EXIT_CODE=$( cat ${EXIT_FILE} )

        if [[ ${EXIT_CODE} != 0 ]];
        then
            HAS_ANY_TEST_FAILED=1
        fi
    done

    if [ ${HAS_ANY_TEST_FAILED} != 0 ];
    then
        exit 1
    fi
}

function system_test() {
    local MOLECULE_SCENARIOS=()

    local MOLECULE_CACHE_DIR=${CACHE_DIR_ABS}/molecule
    local MOLECULE_ROLE_DIR=${PROJECT_DIR_ABS}/ansible/appio-system-tests.role

    rm \
    --force \
    --recursive \
    ${MOLECULE_CACHE_DIR}

    mkdir \
    --parents \
    ${MOLECULE_CACHE_DIR}

    cd \
    ${MOLECULE_ROLE_DIR}

    instantiate_scenarios \
    ${MOLECULE_CACHE_DIR}

    cd \
    ${PROJECT_DIR}

    while pgrep molecule > /dev/null 2>&1;
    do
        inspect_scenarios \
        ${MOLECULE_CACHE_DIR}

        sleep \
        30
    done

    inspect_scenarios \
    ${MOLECULE_CACHE_DIR} \
    --show-log-on-error

    inspect_scenarios \
    ${MOLECULE_CACHE_DIR}
 
    fail_if_any_scenario_failed \
    ${MOLECULE_CACHE_DIR}
}

system_test ${@}
