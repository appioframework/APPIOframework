#!/bin/bash

function __() {
    echo ""
}

function __done() {
    echo "\033[0;32mdone\033[0m"
}

function print_stage() {
    set +u
    echo -e " * ${1} ... $( __${2} )"
    set -u
}

function print_title() {
    set +u
    echo -e "${1} ... $( __${2} )"
    set -u
}
