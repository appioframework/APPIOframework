#!/bin/bash

function precondition_oppo_log_file_is_not_existent() {
    if [ -f "./oppo.log" ];
    then
        echo "the log file already exists, terminating integration tests ..."
        exit 1
    fi
}

function check_for_exisiting_oppo_log_file() {
    if [ ! -f "./oppo.log" ];
    then
        echo "no log entry was created ..."
        exit 1
    fi
}

function check_for_exisiting_file_named() {
    if [ ! -f "${1}" ];
    then
        echo "${2} ..."
        exit 1
    fi
}