#!/bin/bash

set -euo pipefail

echo "Building..."
/bin/bash bash-gitlab-ci/run-build.sh

echo "Publishing..."
/bin/bash bash-gitlab-ci/run-publish.sh

echo "Building installer..."
/bin/bash bash-gitlab-ci/run-build-installer.sh
