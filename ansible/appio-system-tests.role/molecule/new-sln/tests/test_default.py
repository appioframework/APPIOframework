import pytest
import os

import testinfra.utils.ansible_runner

from .util.prepare import assert_that_files_are_existing
from .util.prepare import assert_that_files_are_missing
from .util.prepare import prepare_provide_test_directory

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']
).get_hosts('all')


@pytest.mark.parametrize('case, command, sln_file_name', [
    ['1', 'appio new sln --name test-solution', 'test-solution.appiosln'],
    ['2', 'appio new sln -n     test-solution', 'test-solution.appiosln'],
])
def test_that_appio_new_sln_is_succeeding(host, case, command, sln_file_name):
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
        test_dir_path + sln_file_name,
    ]

    # arrange
    assert_that_files_are_missing(host, file_paths)

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc == 0
    assert appio.stdout != ''

    assert_that_files_are_existing(host, file_paths)


@pytest.mark.parametrize('case, command', [
    ['1f', 'appio new sln --name test/-solution'],
    ['2f', 'appio new sln -n     test/-solution'],
    ['3f', 'appio new sln --name'],
    ['4f', 'appio new sln -n'],
    ['5f', 'appio new sln --exit'],
    ['6f', 'appio new sln -x'],
])
def test_that_appio_new_sln_is_failing(host, case, command):
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log'
    ]

    # arrange
    assert_that_files_are_missing(host, file_paths)

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc != 0
    assert appio.stdout != ''

    assert_that_files_are_existing(host, file_paths)
