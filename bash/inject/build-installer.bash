#!/bin/bash

set -euo pipefail

source bash/config/cache-dir.bash
source bash/config/installer-dir.bash
source bash/config/publish-dir.bash

source bash/config/appio-terminal.bash
source bash/config/open62541.bash

function build_installer_for_open62541() {
    local WORKING_DIR=$( pwd )
    local SRC_DIR=${INSTALLER_SRC_DIR}/${OPEN62541_VERSION}
    local BUILD_DIR=${INSTALLER_DIR}/${OPEN62541_VERSION}

    mkdir \
    --parents \
    $( dirname ${INSTALLER_DIR} )

    cp \
    --archive \
    ${SRC_DIR} \
    ${BUILD_DIR}

    chmod \
    --recursive \
    0755 \
    ${BUILD_DIR}/DEBIAN

    cd \
    $( dirname ${BUILD_DIR} )

    dpkg \
    --build \
    ${OPEN62541_VERSION}

    cd \
    ${WORKING_DIR}
}

function build_installer_for_appio_terminal() {
    local WORKING_DIR=$( pwd )
    local SRC_DIR=${INSTALLER_SRC_DIR}/${APPIO_TERMINAL_VERSION}
    local BUILD_DIR=${INSTALLER_DIR}/${APPIO_TERMINAL_VERSION}

    mkdir \
    --parents \
    $( dirname ${INSTALLER_DIR} )

    cp \
    --archive \
    ${SRC_DIR} \
    ${BUILD_DIR}

    chmod \
    --recursive \
    0755 \
    ${BUILD_DIR}/DEBIAN

    cp \
    ${PUBLISH_DIR}/* \
    ${BUILD_DIR}/usr/bin

    rm \
    --force \
    --recursive \
    ${BUILD_DIR}/usr/bin/*.pdb

    cd \
    $( dirname ${BUILD_DIR} )

    dpkg \
    --build \
    ${APPIO_TERMINAL_VERSION}

    cd \
    ${WORKING_DIR}
}

function build_installer() {
    build_installer_for_open62541
    build_installer_for_appio_terminal
}

build_installer
