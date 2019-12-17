#!/bin/bash

set -euo pipefail

function check_ansible() {
    which \
    ansible

    ansible \
    --version

    ansible \
    --help
}

check_ansible ${@}
