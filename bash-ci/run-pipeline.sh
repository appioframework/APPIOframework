#!/bin/bash

set -euo pipefail

CI_ARTIFACT_DIR="./artifacts"

function run_stage_0() {
    local CI_JOB_ID=${1}

    sudo docker exec ${CI_JOB_ID} /bin/bash bash-ci/run-dotnet-test.sh
    sudo docker exec ${CI_JOB_ID} /bin/bash bash-ci/run-dotnet-build.sh
}

function run_stage_1() {
    local CI_JOB_ID=${1}

    sudo docker exec ${CI_JOB_ID} /bin/bash bash-ci/run-dotnet-publish.sh
}

function run_stage_2() {
    local CI_JOB_ID=${1}

    sudo docker exec ${CI_JOB_ID} mv ${CI_ARTIFACT_DIR}/publish .
    sudo docker exec ${CI_JOB_ID} /bin/bash bash-ci/run-dpkg-build--open62541--v0.3.0.sh
    sudo docker exec ${CI_JOB_ID} /bin/bash bash-ci/run-dpkg-build--appio-terminal.sh
}

function run_pipeline_using_docker() {
    local CI_JOB_WORKDIR="/APPIOframework"

    local CI_TEST_NAMES[0]="Build and test dotnet application"
    local CI_TEST_IDS[0]="build-and-test"
    local CI_TEST_IMAGES[0]="appioframework/dotnet-core:v2.1-sdk"
    local CI_JOB_ARTIFACTS[0]=""

    local CI_TEST_NAMES[1]="Publish dotnet application"
    local CI_TEST_IDS[1]="publish"
    local CI_TEST_IMAGES[1]="appioframework/dotnet-core:v2.1-sdk"
    local CI_JOB_ARTIFACTS[1]="publish"

    local CI_TEST_NAMES[2]="Build debian installer"
    local CI_TEST_IDS[2]="build-debian-installer"
    local CI_TEST_IMAGES[2]="ubuntu:bionic"
    local CI_JOB_ARTIFACTS[2]="installer/open62541--v0.3.0.deb installer/appio-terminal.deb"

    for INDEX in ${!CI_TEST_NAMES[@]}
    do
        local CI_JOB_NAME="${CI_TEST_NAMES[INDEX]}"
        local CI_JOB_ID_PREFIX="stage-${INDEX}"
        local CI_JOB_ID="${CI_JOB_ID_PREFIX}-${CI_TEST_IDS[INDEX]}"
        local CI_JOB_IMAGE="${CI_TEST_IMAGES[INDEX]}"
        local CI_JOB_SCRIPT="run_stage_${INDEX}"
        local CI_JOB_ARTIFACT="${CI_JOB_ARTIFACTS[INDEX]}"

        echo "${CI_JOB_NAME} ..."

        set +e
        if [ $( sudo docker ps --all --quiet --filter name=${CI_JOB_ID} ) ];
        then
            sudo docker kill ${CI_JOB_ID} &> /dev/null
            sudo docker rm ${CI_JOB_ID} > /dev/null
        fi
        set -e

        sudo docker run --detach --name=${CI_JOB_ID} --workdir=${CI_JOB_WORKDIR} ${CI_JOB_IMAGE} sleep infinity > /dev/null

        sudo docker cp --archive . ${CI_JOB_ID}:${CI_JOB_WORKDIR}

        ${CI_JOB_SCRIPT} ${CI_JOB_ID}

        for ARTIFACT in ${CI_JOB_ARTIFACT}
        do
            if [ "${ARTIFACT}" != "" ];
            then
                sudo docker cp --archive ${CI_JOB_ID}:${CI_JOB_WORKDIR}/${ARTIFACT} ${CI_ARTIFACT_DIR}/${ARTIFACT}
            fi
        done

        sudo docker stop ${CI_JOB_ID} | xargs sudo docker rm &> /dev/null

        echo "${CI_JOB_NAME} ... done"
    done
}

function run_pipeline_using_ansible() {
    local CI_JOB_NAME="Run system tests (via ansible)"

    echo "${CI_JOB_NAME} ..."

    rsync --archive ${CI_ARTIFACT_DIR}/installer .

    /bin/bash bash-ci/run-system-tests.sh

    echo "${CI_JOB_NAME} ... done"
}

function run_pipeline() {
    run_pipeline_using_docker
    run_pipeline_using_ansible
}

run_pipeline
