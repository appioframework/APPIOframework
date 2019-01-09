#!/bin/bash

set -euo pipefail

/bin/bash bash-gitlab-ci/run-build.sh
/bin/bash bash-gitlab-ci/run-publish.sh
/bin/bash bash-gitlab-ci/run-installer.sh
