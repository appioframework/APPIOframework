#!/bin/bash

set -euo pipefail

function export_askpass_gui_prompt_for_sudo() {
    export SUDO_ASKPASS="$( which ssh-askpass )"
}

function elevate() {
    local TARGET_USER=${1}
    local TARGET_BINARY=${2}
    local TARGET_COMMAND=${3}

    export_askpass_gui_prompt_for_sudo

    if [ ${USER} != ${TARGET_USER} ];
    then
        sudo \
        --askpass \
        --group ${TARGET_USER} \
        --user ${TARGET_USER} \
        ${TARGET_BINARY} \
        ${TARGET_COMMAND} ${@:4}
    else
        ${TARGET_BINARY} \
        ${TARGET_COMMAND} ${@:4}
    fi
}

elevate ${@}
