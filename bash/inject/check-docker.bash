#!/bin/bash

set -euo pipefail

function check_docker() {
    which \
    docker

    docker \
    run \
    hello-world

    docker \
    --help
}

check_docker ${@}
