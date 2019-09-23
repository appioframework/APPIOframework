import pytest
import os

import testinfra.utils.ansible_runner

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']
).get_hosts('all')


def prepare_provide_test_directory(case):
    test_dir_path = case + '/'

    mk_test_dir = host.run('mkdir --parents ' + test_dir_path)

    assert mk_test_dir.rc == 0

    test_dir = host.file(test_dir_path)

    assert test_dir.exists
    assert test_dir.is_directory

    return test_dir_path


@pytest.mark.parametrize('case, command', [
    ['1', 'appio --help'],
    ['2', 'appio -h'],
    ['3', 'appio help'],
    ['4', 'appio ?'],
    ['5', 'appio'],
])
def test_that_appio_help_is_succeeding(host, case, command):
    # prepare
    test_dir_path = prepare_provide_test_directory(case)
    command_line = 'cd ' + test_dir_path + ' && ' + command

    # arrange
    log_file_path = test_dir_path + 'appio.log'
    log_file = host.file(log_file_path)
    assert not log_file.exists

    # act
    appio = host.run(command_line)

    # assert
    assert appio.rc == 0
    assert appio.stdout != ''
    log_file = host.file(log_file_path)
    assert log_file.exists
