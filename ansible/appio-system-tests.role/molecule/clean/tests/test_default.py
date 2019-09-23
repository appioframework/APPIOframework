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


@pytest.mark.parametrize('case, command, app_name', [
    ['1', 'appio clean --name my-app', 'my-app'],
    ['2', 'appio clean -n     my-app', 'my-app'],
])
def test_that_appio_clean_is_succeeding(host, case, command, app_name):
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    log_file_path = test_dir_path + 'appio.log'
    build_dir_path = test_dir_path + app_name + '/build'

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',
        'appio build -n ' + app_name,
        'rm -f appio.log',
    ):
        prepare = host.run('cd ' + test_dir_path + ' && ' + prepare_command)

        assert prepare.rc == 0

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

    build_dir = host.file(build_dir_path)
    assert not build_dir.exists


@pytest.mark.parametrize('case, command', [
    ['1f', 'appio clean --name my-app-5263452364'],
    ['2f', 'appio clean -n     my-app-5263452364'],
    ['3f', 'appio clean --name my/-app'],
    ['4f', 'appio clean -n     my/-app'],
    ['5f', 'appio clean --name'],
    ['6f', 'appio clean -n'],
    ['7f', 'appio clean --exit'],
    ['8f', 'appio clean -x'],
])
def test_that_appio_clean_is_failing(host, case, command):
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
