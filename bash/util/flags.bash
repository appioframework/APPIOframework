#!/bin/bash

FLAG_BUILD_AND_TEST="--build-and-test"
FLAG_FULL_PIPELINE="--full-pipeline"
FLAG_PUBLISH="--publish"

function are_empty_args() {
    if [ ${#} = 0 ];
    then
        exit 0
    else
        exit 1
    fi
}

function should_run_build_and_test() {
    if $( are_empty_args ${@} );
    then
        exit 0
    fi

    for ARG in ${@}
    do
        if [ "${ARG}" == "${FLAG_BUILD_AND_TEST}" ] || [ "${ARG}" == "${FLAG_FULL_PIPELINE}" ];
        then
            exit 0
        fi
    done

    exit 1
}

function should_run_publish() {
    if $( are_empty_args ${@} );
    then
        exit 0
    fi

    for ARG in ${@}
    do
        if [ "${ARG}" == "${FLAG_PUBLISH}" ] || [ "${ARG}" == "${FLAG_FULL_PIPELINE}" ];
        then
            exit 0
        fi
    done

    exit 1
}

function print_condition_for_build_and_test() {
    echo "${FLAG_BUILD_AND_TEST} or ${FLAG_FULL_PIPELINE}"
}

function print_condition_for_publish() {
    echo "${FLAG_PUBLISH} or ${FLAG_FULL_PIPELINE}"
}
