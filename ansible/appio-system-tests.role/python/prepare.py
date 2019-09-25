import pytest
import os

import testinfra.utils.ansible_runner


def assert_that_files_are_executable(host, file_paths):
    for file_path in file_paths:
        f = host.file(file_path)

        assert f.mode == 0o755


def assert_that_files_are_existing(host, file_paths):
    for file_path in file_paths:
        f = host.file(file_path)

        assert f.exists


def assert_that_files_are_missing(host, file_paths):
    for file_path in file_paths:
        f = host.file(file_path)

        assert not f.exists


def prepare_provide_test_directory(host, case):
    test_dir_path = case + '/'

    mk_test_dir = host.run('mkdir --parents ' + test_dir_path)

    assert mk_test_dir.rc == 0

    test_dir = host.file(test_dir_path)

    assert test_dir.exists
    assert test_dir.is_directory

    return test_dir_path
