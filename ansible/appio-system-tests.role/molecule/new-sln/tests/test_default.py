import pytest
import os

import testinfra.utils.ansible_runner

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']
).get_hosts('all')


def prepare_provide_test_directory(host, case):
    test_dir_path = case + '/'

    mk_test_dir = host.run('mkdir --parents ' + test_dir_path)

    assert mk_test_dir.rc == 0

    test_dir = host.file(test_dir_path)

    assert test_dir.exists
    assert test_dir.is_directory

    return test_dir_path


@pytest.mark.parametrize('case, command, sln_file_name', [
    ['1', 'appio new sln --name test-solution', 'test-solution.appiosln'],
    ['2', 'appio new sln -n     test-solution', 'test-solution.appiosln'],
])
def test_that_appio_new_sln_is_succeeding(host, case, command, sln_file_name):
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    log_file_path = test_dir_path + 'appio.log'
    sln_file_path = test_dir_path + sln_file_name

    # arrange
    log_file = host.file(log_file_path)
    assert not log_file.exists

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc == 0
    assert appio.stdout != ''

    log_file = host.file(log_file_path)
    assert log_file.exists

    sln_file = host.file(sln_file_path)
    assert sln_file.exists


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

    log_file_path = test_dir_path + 'appio.log'

    # arrange
    log_file = host.file(log_file_path)
    assert not log_file.exists

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc != 0
    assert appio.stdout != ''

    log_file = host.file(log_file_path)
    assert log_file.exists
