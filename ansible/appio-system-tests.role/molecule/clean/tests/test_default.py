import pytest
import os

import testinfra.utils.ansible_runner

from .util.prepare import assert_that_files_are_existing
from .util.prepare import assert_that_files_are_missing
from .util.prepare import prepare_provide_test_directory

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']
).get_hosts('all')


@pytest.mark.parametrize('case, command', [
    ['1_help', 'appio clean --help'],
    ['2_help', 'appio clean -h'],
    ['3_help', 'appio clean'],
])
def test_that_appio_clean_help_is_succeeding(host, case, command):
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    # arrange
    assert_that_files_are_missing(host, file_paths)

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc == 0
    assert appio.stdout != ''

    assert_that_files_are_existing(host, file_paths)


@pytest.mark.parametrize('case, command, app_name', [
    ['1', 'appio clean --name my-app', 'my-app'],
    ['2', 'appio clean -n     my-app', 'my-app'],
])
def test_that_appio_clean_is_succeeding(host, case, command, app_name):
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    removed_file_paths = [
        test_dir_path + app_name + '/build',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'appio build -n ' + app_name,
        'rm -f appio.log',
    ):
        prepare = host.run('cd ' + test_dir_path + ' && ' + prepare_command)

        assert prepare.rc == 0

    # arrange
    assert_that_files_are_missing(host, file_paths)
    assert_that_files_are_existing(host, removed_file_paths)

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc == 0
    assert appio.stdout != ''

    assert_that_files_are_existing(host, file_paths)
    assert_that_files_are_missing(host, removed_file_paths)


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

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    # arrange
    assert_that_files_are_missing(host, file_paths)

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc != 0
    assert appio.stdout != ''

    assert_that_files_are_existing(host, file_paths)
