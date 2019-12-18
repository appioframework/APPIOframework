#!/bin/bash

FLAG_BUILD_AND_TEST="--build-and-test"
FLAG_BUILD_INSTALLER="--build-installer"
FLAG_FULL_PIPELINE="--full-pipeline"
FLAG_HELP="--help"
FLAG_INSTALL="--install"
FLAG_MOLECULE="--molecule"
FLAG_PUBLISH="--publish"
FLAG_REMOVE="--remove"

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

function should_run_build_installer() {
    if $( are_empty_args ${@} );
    then
        exit 0
    fi

    for ARG in ${@}
    do
        if [ "${ARG}" == "${FLAG_BUILD_INSTALLER}" ] || [ "${ARG}" == "${FLAG_FULL_PIPELINE}" ];
        then
            exit 0
        fi
    done

    exit 1
}

function should_run_help() {
    if $( are_empty_args ${@} );
    then
        exit 0
    fi

    for ARG in ${@}
    do
        if [ "${ARG}" == "${FLAG_HELP}" ] || [ "${ARG}" == "${FLAG_FULL_PIPELINE}" ];
        then
            exit 0
        fi
    done

    exit 1
}

function should_run_install() {
    if $( are_empty_args ${@} );
    then
        exit 0
    fi

    for ARG in ${@}
    do
        if [ "${ARG}" == "${FLAG_INSTALL}" ] || [ "${ARG}" == "${FLAG_FULL_PIPELINE}" ];
        then
            exit 0
        fi
    done

    exit 1
}

function should_run_molecule() {
    if $( are_empty_args ${@} );
    then
        exit 0
    fi

    for ARG in ${@}
    do
        if [ "${ARG}" == "${FLAG_MOLECULE}" ] || [ "${ARG}" == "${FLAG_FULL_PIPELINE}" ];
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

function should_run_remove() {
    if $( are_empty_args ${@} );
    then
        exit 0
    fi

    for ARG in ${@}
    do
        if [ "${ARG}" == "${FLAG_REMOVE}" ] || [ "${ARG}" == "${FLAG_FULL_PIPELINE}" ];
        then
            exit 0
        fi
    done

    exit 1
}

function print_condition_for_build_and_test() {
    echo "${FLAG_BUILD_AND_TEST} or ${FLAG_FULL_PIPELINE}"
}

function print_condition_for_build_installer() {
    echo "${FLAG_BUILD_INSTALLER} or ${FLAG_FULL_PIPELINE}"
}

function print_condition_for_help() {
    echo "${FLAG_HELP} or ${FLAG_FULL_PIPELINE}"
}

function print_condition_for_install() {
    echo "${FLAG_INSTALL} or ${FLAG_FULL_PIPELINE}"
}

function print_condition_for_molecule() {
    echo "${FLAG_MOLECULE} or ${FLAG_FULL_PIPELINE}"
}

function print_condition_for_publish() {
    echo "${FLAG_PUBLISH} or ${FLAG_FULL_PIPELINE}"
}

function print_condition_for_remove() {
    echo "${FLAG_REMOVE} or ${FLAG_FULL_PIPELINE}"
}
