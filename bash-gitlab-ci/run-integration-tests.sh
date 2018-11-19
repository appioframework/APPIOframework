#!/bin/bash

set -euo pipefail

echo "Testing build-help--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-build-help--success.sh -
echo "Testing build-help--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-build-help--success.sh verbose

echo "Testing build-opcuaapp--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-build-opcuaapp--success.sh -
echo "Testing build-opcuaapp--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-build-opcuaapp--success.sh verbose
echo "Testing build-opcuaapp--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-build-opcuaapp--failure.sh

echo "Testing clean-opcuaapp--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-clean-opcuaapp--success.sh -
echo "Testing clean-opcuaapp--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-clean-opcuaapp--success.sh verbose

echo "Testing clean-help--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-clean-help--success.sh -
echo "Testing clean-help--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-clean-help--success.sh verbose

echo "Testing hello--success"
/bin/bash bash-gitlab-ci/integration-tests/run-hello--success.sh

echo "Testing help--success"
/bin/bash bash-gitlab-ci/integration-tests/run-help--success.sh

echo "Testing new-help--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-new-help--success.sh -
echo "Testing new-help--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-new-help--success.sh verbose

echo "Testing new-opcuaapp--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-new-opcuaapp--success.sh -
echo "Testing new-opcuaapp--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-new-opcuaapp--success.sh verbose
echo "Testing new-opcuaapp--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-new-opcuaapp--failure.sh

echo "Testing new-sln--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-new-sln--success.sh -
echo "Testing new-sln--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-new-sln--success.sh verbose
echo "Testing new-sln--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-new-sln--failure.sh

echo "Testing publish-help--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-publish-help--success.sh -
echo "Testing publish-help--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-publish-help--success.sh verbose

echo "Testing publish-opcuaapp--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-publish-opcuaapp--success.sh -
echo "Testing publish-opcuaapp--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-publish-opcuaapp--success.sh verbose
echo "Testing publish-opcuaapp--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-publish-opcuaapp--failure.sh

echo "Testing version--success"
/bin/bash bash-gitlab-ci/integration-tests/run-version--success.sh