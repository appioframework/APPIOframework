import pytest
import os

import testinfra.utils.ansible_runner

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']
).get_hosts('all')


@pytest.mark.parametrize('case, command', [
    ['1', 'appio new sln --name test-solution'],
    ['2', 'appio new sln -n     test-solution'],
])
def test_that_appio_new_sln_is_succeeding(host, case, command):
    # prepare
    test_dir_path = case + '/'

    mk_test_dir = host.run('mkdir --parents ' + test_dir_path)

    assert mk_test_dir.rc == 0

    test_dir = host.file(test_dir_path)

    assert test_dir.exists
    assert test_dir.is_directory

    # arrange
    log_file = host.file(test_dir_path + 'appio.log')
    assert not log_file.exists

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc == 0
    assert appio.stdout != ''
    log_file = host.file(test_dir_path + 'appio.log')
    assert log_file.exists
    sln_file = host.file(test_dir_path + 'test-solution.appiosln')
    assert sln_file.exists


@pytest.mark.parametrize('case, command', [
    ['1f', 'appio new sln --name test\\\\-solution'],
    ['2f', 'appio new sln -n     test\\\\-solution'],
    ['3f', 'appio new sln --name'],
    ['4f', 'appio new sln -n'],
    ['5f', 'appio new sln --exit'],
    ['6f', 'appio new sln -x'],
])
def test_that_appio_new_sln_is_failing(host, case, command):
    # prepare
    test_dir_path = case + '/'

    mk_test_dir = host.run('mkdir --parents ' + test_dir_path)

    assert mk_test_dir.rc == 0

    test_dir = host.file(test_dir_path)

    assert test_dir.exists
    assert test_dir.is_directory

    # arrange
    log_file = host.file(test_dir_path + 'appio.log')
    assert not log_file.exists

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc != 0
    assert appio.stdout != ''
    log_file = host.file(test_dir_path + 'appio.log')
    assert log_file.exists
