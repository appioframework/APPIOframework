#!/bin/bash

set -euo pipefail

function run_molecule() {
    cd \
    ansible/appio-system-tests.role/

    molecule \
    test \
    --all
}

run_molecule ${@}
