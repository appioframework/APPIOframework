#!/bin/bash

set -euo pipefail

source bash/util/functions.bash

function run_reinstall_appio_terminal() {
    local TITLE="Reinstalling local working progress of APPIO"

    print_title \
    "${TITLE}"

    begin_observe_exit_code
    /bin/bash \
    bash/ci/run-job-publish.bash ${@}
    end_observe_exit_code

    begin_observe_exit_code
    /bin/bash \
    bash/ci/run-job-build-installer.bash ${@}
    end_observe_exit_code

    begin_observe_exit_code
    /bin/bash \
    bash/util/elevate.bash \
    root \
    /bin/bash \
    bash/inject/remove.bash
    end_observe_exit_code

    begin_observe_exit_code
    /bin/bash \
    bash/util/elevate.bash \
    root \
    /bin/bash \
    bash/inject/install.bash
    end_observe_exit_code

    print_title \
    "${TITLE}" \
    "${DID_FAIL}"
}

run_reinstall_appio_terminal ${@}
