#!/bin/bash

set -euo pipefail

cd ansible/appio-system-tests.role/

molecule test --all
