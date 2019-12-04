#!/bin/bash

set -euo pipefail

echo "Building appio-terminal ..."
/bin/bash bash-ci/run-dotnet-build.sh
echo "Building appio-terminal ... done"

echo "Publishing appio-terminal ..."
/bin/bash bash-ci/run-dotnet-publish.sh
echo "Publishing appio-terminal ... done"

echo "Building installer for open62541--v1.0.0 ..."
/bin/bash bash-ci/run-dpkg-build--open62541--v1.0.0.sh
echo "Building installer for open62541--v1.0.0 ... done"

echo "Building installer for appio-terminal ..."
/bin/bash bash-ci/run-dpkg-build--appio-terminal.sh
echo "Building installer for appio-terminal ... done"

echo "Removing appio-terminal ..."
/bin/bash bash-developer/util-elevate.sh root bash-ci/run-dpkg-remove--appio-terminal.sh
echo "Removing appio-terminal ... done"

echo "Removing open62541--v1.0.0 ..."
/bin/bash bash-developer/util-elevate.sh root bash-ci/run-dpkg-remove--open62541--v1.0.0.sh
echo "Removing open62541--v1.0.0 ... done"

echo "Installing open62541--v1.0.0 ..."
/bin/bash bash-developer/util-elevate.sh root bash-ci/run-dpkg-install--open62541--v1.0.0.sh
echo "Installing open62541--v1.0.0 ... done"

echo "Installing appio-terminal ..."
/bin/bash bash-developer/util-elevate.sh root bash-ci/run-dpkg-install--appio-terminal.sh
echo "Installing appio-terminal ... done"
