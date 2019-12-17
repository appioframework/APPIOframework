#!/bin/bash

set -euo pipefail

function check_molecule() {
    which \
    molecule

    molecule \
    --version

    molecule \
    --help
}

check_molecule ${@}
