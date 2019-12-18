#!/bin/bash

set -euo pipefail

function system_test() {
    cd \
    ansible/appio-system-tests.role/

    molecule \
    test \
    --all
}

system_test ${@}
