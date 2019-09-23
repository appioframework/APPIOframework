#!/bin/bash

set -euo pipefail

# echo "Testing build-help--success"
# /bin/bash bash-gitlab-ci/integration-tests/run-build-help--success.sh

# echo "Testing build-opcuaapp--success"
# /bin/bash bash-gitlab-ci/integration-tests/run-build-opcuaapp--success.sh
# echo "Testing build-opcuaapp--failure"
# /bin/bash bash-gitlab-ci/integration-tests/run-build-opcuaapp--failure.sh
# echo "Testing build-opcuaapp--failure--extended"
# /bin/bash bash-gitlab-ci/integration-tests/run-build-opcuaapp--failure--extended.sh

echo "Testing clean-help--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-clean-help--success.sh -
echo "Testing clean-help--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-clean-help--success.sh verbose

echo "Testing clean-opcuaapp--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-clean-opcuaapp--success.sh -
echo "Testing clean-opcuaapp--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-clean-opcuaapp--success.sh verbose
echo "Testing clean-opcuaapp--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-clean-opcuaapp--failure.sh
echo "Testing clean-opcuaapp--failure--extended"
/bin/bash bash-gitlab-ci/integration-tests/run-clean-opcuaapp--failure--extended.sh

echo "Testing deploy-help--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-deploy-help--success.sh -
echo "Testing deploy-help--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-deploy-help--success.sh verbose

echo "Testing deploy-opcuaapp--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-deploy-opcuaapp--success.sh -
echo "Testing deploy-opcuaapp--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-deploy-opcuaapp--success.sh verbose
echo "Testing deploy-opcuaapp--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-deploy-opcuaapp--failure.sh
echo "Testing deploy-opcuaapp--failure--extended"
/bin/bash bash-gitlab-ci/integration-tests/run-deploy-opcuaapp--failure--extended.sh

echo "Testing hello--success"
/bin/bash bash-gitlab-ci/integration-tests/run-hello--success.sh

# echo "Testing help--success"
# /bin/bash bash-gitlab-ci/integration-tests/run-help--success.sh

echo "Testing new-help--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-new-help--success.sh -
echo "Testing new-help--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-new-help--success.sh verbose

echo "Testing new-opcuaapp-ClientServer--success -"
/bin/bash bash-gitlab-ci/integration-tests/run-new-opcuaapp-ClientServer--success.sh -
echo "Testing new-opcuaapp-ClientServer--success verbose"
/bin/bash bash-gitlab-ci/integration-tests/run-new-opcuaapp-ClientServer--success.sh verbose
echo "Testing new-opcuaapp-Client--success"
/bin/bash bash-gitlab-ci/integration-tests/run-new-opcuaapp-Client--success.sh
echo "Testing new-opcuaapp-Server--success"
/bin/bash bash-gitlab-ci/integration-tests/run-new-opcuaapp-Server--success.sh
echo "Testing new-opcuaapp--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-new-opcuaapp--failure.sh

# echo "Testing new-sln--success -"
# /bin/bash bash-gitlab-ci/integration-tests/run-new-sln--success.sh -
# echo "Testing new-sln--success verbose"
# /bin/bash bash-gitlab-ci/integration-tests/run-new-sln--success.sh verbose
# echo "Testing new-sln--failure"
# /bin/bash bash-gitlab-ci/integration-tests/run-new-sln--failure.sh

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

# echo "Testing version--success"
# /bin/bash bash-gitlab-ci/integration-tests/run-version--success.sh

echo "Testing import information-model--success"
/bin/bash bash-gitlab-ci/integration-tests/run-import-information-model--success.sh
echo "Testing import information-model--success--extended"
/bin/bash bash-gitlab-ci/integration-tests/run-import-information-model--success--extended.sh
echo "Testing import information-model--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-import-information-model--failure.sh
echo "Testing import information-model sample--success"
/bin/bash bash-gitlab-ci/integration-tests/run-import-information-model-sample--success.sh
echo "Testing import information-model sample--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-import-information-model-sample--failure.sh
echo "Testing import--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-import--failure.sh

echo "Testing generate-help--success"
/bin/bash bash-gitlab-ci/integration-tests/run-generate-help--success.sh
echo "Testing generate-help--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-generate-help--failure.sh
echo "Testing generate--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-generate--failure.sh
echo "Testing generate-information-model--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-generate-information-model--failure.sh
echo "Testing generate-information-model--success"
/bin/bash bash-gitlab-ci/integration-tests/run-generate-information-model--success.sh

echo "Testing sln-help--success"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-help--success.sh
echo "Testing sln-add--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-add--failure.sh
echo "Testing sln-add--success"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-add--success.sh
echo "Testing sln-remove--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-remove--failure.sh
echo "Testing sln-remove--success"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-remove--success.sh
echo "Testing sln-build--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-build--failure.sh
echo "Testing sln-build--failure--extended"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-build--failure--extended.sh
echo "Testing sln-build--success"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-build--success.sh
echo "Testing sln-publish-failure"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-publish--failure.sh
echo "Testing sln-publish--failure--extended"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-publish--failure--extended.sh
echo "Testing sln-publish--success"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-publish--success.sh
echo "Testing sln-deploy-failure"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-deploy--failure.sh
echo "Testing sln-deploy--failure--extended"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-deploy--failure--extended.sh
echo "Testing sln-deploy--success"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-deploy--success.sh

echo "Testing reference-add--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-reference-add--failure.sh
echo "Testing reference-add--success"
/bin/bash bash-gitlab-ci/integration-tests/run-reference-add--success.sh
echo "Testing reference-remove--failure"
/bin/bash bash-gitlab-ci/integration-tests/run-reference-remove--failure.sh
echo "Testing reference-remove--success"
/bin/bash bash-gitlab-ci/integration-tests/run-reference-remove--success.sh