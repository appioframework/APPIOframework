#!/bin/bash

set -euo pipefail

source bash/util/functions.bash

function check_sdk_dependencies() {
    local TITLE="Checking APPIO development dependencies"

    print_title \
    "${TITLE}"

    begin_observe_exit_code
    /bin/bash \
    bash/development/check-job-docker.bash ${@}
    end_observe_exit_code

    begin_observe_exit_code
    /bin/bash \
    bash/development/check-job-ansible.bash ${@}
    end_observe_exit_code

    begin_observe_exit_code
    /bin/bash \
    bash/development/check-job-molecule.bash ${@}
    end_observe_exit_code

    begin_observe_exit_code
    /bin/bash \
    bash/development/check-job-dotnet-sdk.bash ${@}
    end_observe_exit_code

    print_title \
    "${TITLE}" \
    "${DID_FAIL}"

    exit ${DID_FAIL}
}

check_sdk_dependencies ${@}
