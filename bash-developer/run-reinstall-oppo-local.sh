#!/bin/bash

set -euo pipefail

/bin/bash bash-developer/run-build-installer-local.sh

echo "Purging..."
/bin/bash bash-developer/util-elevate.sh root bash-gitlab-ci/run-purge.sh

echo "Installing..."
/bin/bash bash-developer/util-elevate.sh root bash-gitlab-ci/run-installer.sh
