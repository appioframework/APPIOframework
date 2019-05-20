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
        echo "${2}"
        exit 1
    fi
}

function check_for_missing_file_named() {
    if [ -f "${1}" ];
    then
        echo "${2}"
        exit 1
    fi
}

function check_for_exisiting_directory_named() {
    if [ ! -d "${1}" ];
    then
        echo "${2}"
        exit 1
    fi
}

function check_for_missing_directory_named() {
    if [ -d "${1}" ];
    then
        echo "${2}"
        exit 1
    fi
}

function check_for_executable_file() {
    if [[ ! -x "${1}" ]];
    then
        echo "${2}"
        exit 1
    fi
}

function check_for_non_zero_error_code() {
    if [ ${?} = 0 ];
    then
        echo "failing command did not result in exit code != 0 ..."
        exit 1
    fi
}

function generate_crypto_all_formats() {
    echo "generating manual certificate"
    openssl req -new -x509 -out mycert.der -outform der -nodes -keyout mykey.key -days 90 -subj '/CN=example.com/O=MyOrg/C=DE'
    openssl rsa -outform der -in mykey.key -out mykey.der
    openssl x509 -inform der -outform pem -in mycert.der -out mycert.pem
}