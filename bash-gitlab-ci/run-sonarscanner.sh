#!/bin/bash

set -euo pipefail

export PATH="$PATH:/root/.dotnet/tools"                                                                       \
   && dotnet sonarscanner begin                                                                               \
                          /k:"oppo-terminal"                                                                  \
                          /d:sonar.host.url="https://sonarqube.talsen.team"                                   \
                          /d:sonar.login=${SONAR_LOGIN}                                                       \
                          /d:sonar.gitlab.commit_sha=${CI_COMMIT_SHA}                                         \
                          /d:sonar.gitlab.ref_name=${CI_COMMIT_REF_NAME}                                      \
                          /d:sonar.gitlab.project_id=${CI_PROJECT_ID}                                         \
                          /d:sonar.gitlab.failure_notification_mode=exit-code                                 \
                          /d:sonar.cs.opencover.reportsPaths="src/*.tests/coverage.opencover.xml"             \
   && dotnet build oppo-terminal.sln                                                                          \
   && dotnet sonarscanner end                                                                                 \
                          /d:sonar.login=${SONAR_LOGIN}
