import pytest
import os

import testinfra.utils.ansible_runner

from .util.prepare import assert_that_files_are_executable
from .util.prepare import assert_that_files_are_existing
from .util.prepare import assert_that_files_are_missing
from .util.prepare import prepare_provide_test_directory

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']
).get_hosts('all')


@pytest.mark.parametrize('case, command', [
    ['1_help', 'appio publish --help'],
    ['2_help', 'appio publish -h'],
    ['3_help', 'appio publish'],
])
def test_that_appio_publish_help_is_succeeding(host, case, command):
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


@pytest.mark.parametrize('case, command', [
    ['1', 'appio publish --name my-app'],
    ['2', 'appio publish -n     my-app'],
])
def test_that_appio_publish_is_succeeding(host, case, command):
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
        test_dir_path + app_name + 'publish/client-app',
        test_dir_path + app_name + 'publish/server-app',
    ]

    exe_file_paths = [
        file_paths[1],
        file_paths[2],
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

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc == 0
    assert appio.stdout != ''

    assert_that_files_are_existing(host, file_paths)
    assert_that_files_are_executable(host, exe_file_paths)


@pytest.mark.parametrize('case, command', [
    ['1f', 'appio publish --name my/-app'],
    ['2f', 'appio publish -n     my/-app'],
    ['3f', 'appio publish --name'],
    ['4f', 'appio publish -n'],
    ['5f', 'appio publish --exit'],
    ['6f', 'appio publish -x'],
])
def test_that_appio_publish_is_failing(host, case, command):
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
