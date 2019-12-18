#!/bin/bash

DID_FAIL=0
EXIT_CODE=

function __() {
    echo ""
}

function __0() {
    __done
}

function __1() {
    __error
}

function __done() {
    echo "\033[0;32mdone\033[0m"
}

function __error() {
    echo "\033[0;31merror\033[0m"
}

function __skipped() {
    echo "\033[0;33mskipped\033[0m"
}

function begin_observe_exit_code() {
    set +e
}

function end_observe_exit_code() {
    EXIT_CODE=${?}
    if [ ${EXIT_CODE} != 0 ];
    then
        DID_FAIL=1
    fi
    set -e
}

function print_date_now() {
    echo $( date "+%Y-%m-%d %H:%M:%S" )
}

function print_entry_step_sub() {
    set +u
    echo -e "$( print_date_now ):      * ${1} ... $( __${2} )"
    set -u
}

function print_entry_step() {
    set +u
    echo -e "$( print_date_now ):     * ${1} ... $( __${2} )"
    set -u
}

function print_entry() {
    set +u
    echo -e "$( print_date_now ):    * ${1} ... $( __${2} )"
    set -u
}

function print_job() {
    set +u
    echo -e "$( print_date_now ):   * ${1} ... $( __${2} )"
    set -u
}

function print_stage() {
    set +u
    echo -e "$( print_date_now ):  * ${1} ... $( __${2} )"
    set -u
}

function print_title() {
    set +u
    echo -e "$( print_date_now ): ${1} ... $( __${2} )"
    set -u
}
