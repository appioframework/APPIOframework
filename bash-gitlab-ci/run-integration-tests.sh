#!/bin/bash

set -euo pipefail

# echo "Testing generate--failure"
# /bin/bash bash-gitlab-ci/integration-tests/run-generate--failure.sh
# echo "Testing generate-information-model--failure"
# /bin/bash bash-gitlab-ci/integration-tests/run-generate-information-model--failure.sh
# echo "Testing generate-information-model--success"
# /bin/bash bash-gitlab-ci/integration-tests/run-generate-information-model--success.sh

# echo "Testing sln-add--failure"
# /bin/bash bash-gitlab-ci/integration-tests/run-sln-add--failure.sh
# echo "Testing sln-add--success"
# /bin/bash bash-gitlab-ci/integration-tests/run-sln-add--success.sh
# echo "Testing sln-remove--failure"
# /bin/bash bash-gitlab-ci/integration-tests/run-sln-remove--failure.sh
# echo "Testing sln-remove--success"
# /bin/bash bash-gitlab-ci/integration-tests/run-sln-remove--success.sh
# echo "Testing sln-build--failure"
# /bin/bash bash-gitlab-ci/integration-tests/run-sln-build--failure.sh
echo "Testing sln-build--failure--extended"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-build--failure--extended.sh
# echo "Testing sln-build--success"
# /bin/bash bash-gitlab-ci/integration-tests/run-sln-build--success.sh
# echo "Testing sln-publish-failure"
# /bin/bash bash-gitlab-ci/integration-tests/run-sln-publish--failure.sh
echo "Testing sln-publish--failure--extended"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-publish--failure--extended.sh
# echo "Testing sln-publish--success"
# /bin/bash bash-gitlab-ci/integration-tests/run-sln-publish--success.sh
# echo "Testing sln-deploy-failure"
# /bin/bash bash-gitlab-ci/integration-tests/run-sln-deploy--failure.sh
echo "Testing sln-deploy--failure--extended"
/bin/bash bash-gitlab-ci/integration-tests/run-sln-deploy--failure--extended.sh
# echo "Testing sln-deploy--success"
# /bin/bash bash-gitlab-ci/integration-tests/run-sln-deploy--success.sh

# echo "Testing reference-add--failure"
# /bin/bash bash-gitlab-ci/integration-tests/run-reference-add--failure.sh
# echo "Testing reference-add--success"
# /bin/bash bash-gitlab-ci/integration-tests/run-reference-add--success.sh
# echo "Testing reference-remove--failure"
# /bin/bash bash-gitlab-ci/integration-tests/run-reference-remove--failure.sh
# echo "Testing reference-remove--success"
# /bin/bash bash-gitlab-ci/integration-tests/run-reference-remove--success.sh