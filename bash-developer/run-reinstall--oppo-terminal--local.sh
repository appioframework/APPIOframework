#!/bin/bash

set -euo pipefail

echo "Building oppo-terminal ..."
/bin/bash bash-gitlab-ci/run-dotnet-build.sh
echo "Building oppo-terminal ... done"

echo "Publishing oppo-terminal ..."
/bin/bash bash-gitlab-ci/run-dotnet-publish.sh
echo "Publishing oppo-terminal ... done"

echo "Building installer for open62541--v0.3.0 ..."
/bin/bash bash-gitlab-ci/run-dpkg-build--open62541--v0.3.0.sh
echo "Building installer for open62541--v0.3.0 ... done"

echo "Building installer for oppo-terminal ..."
/bin/bash bash-gitlab-ci/run-dpkg-build--oppo-terminal.sh
echo "Building installer for oppo-terminal ... done"

echo "Removing oppo-terminal ..."
/bin/bash bash-developer/util-elevate.sh root bash-gitlab-ci/run-dpkg-remove--oppo-terminal.sh
echo "Removing oppo-terminal ... done"

echo "Removing open62541--v0.3.0 ..."
/bin/bash bash-developer/util-elevate.sh root bash-gitlab-ci/run-dpkg-remove--open62541--v0.3.0.sh
echo "Removing open62541--v0.3.0 ... done"

echo "Installing open62541--v0.3.0 ..."
/bin/bash bash-developer/util-elevate.sh root bash-gitlab-ci/run-dpkg-install--open62541--v0.3.0.sh
echo "Installing open62541--v0.3.0 ... done"

echo "Installing oppo-terminal ..."
/bin/bash bash-developer/util-elevate.sh root bash-gitlab-ci/run-dpkg-install--oppo-terminal.sh
echo "Installing oppo-terminal ... done"
