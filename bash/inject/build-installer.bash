#!/bin/bash

set -euo pipefail

source bash/config/open62541.bash

CACHE_DIR=cache
INSTALLER_DIR=installer

function build_installer_for_open62541() {
    local WORKING_DIR=$( pwd )
    local BUILD_DIR=${CACHE_DIR}/installer/${OPEN62541_VERSION}

    mkdir \
    --parents \
    $( dirname ${BUILD_DIR} )

    cp \
    --archive \
    ${INSTALLER_DIR}/${OPEN62541_VERSION} \
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

function build_installer() {
    build_installer_for_open62541
}

build_installer
