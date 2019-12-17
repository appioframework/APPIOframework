#!/bin/bash

set -euo pipefail

function export_askpass_gui_prompt_for_sudo() {
    export SUDO_ASKPASS="$( which ssh-askpass )"
}

function elevate() {
    VAR_USER_NAME=${1}
    VAR_FILE_APPLICATION_ENVIRONMENT=${2}

    export_askpass_gui_prompt_for_sudo

    sudo \
    --askpass \
    --group ${VAR_USER_NAME} \
    --user ${VAR_USER_NAME} \
    /bin/bash \
    ${2} ${@:3}
}

elevate ${@}
