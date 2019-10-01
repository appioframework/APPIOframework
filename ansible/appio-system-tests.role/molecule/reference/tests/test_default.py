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
    ['1f_add', 'appio reference add -s my-client -c my-server'],
    ['2f_add', 'appio reference add -n my-client -s my-server'],
    ['3f_add', 'appio reference add -c my-client -n my-server'],
    ['4f_add', 'appio reference add -c my-client'],
    ['5f_add', 'appio reference add -s my-server'],
])
def test_that_appio_reference_add_is_failing(host, case, command):
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


@pytest.mark.parametrize('case, command, client_app, server_app', [  # noqa: #501
    ['1_add', 'appio reference add -c my-client -s my-server', 'my-client', 'my-server'],  # noqa: #501
    ['2_add', 'appio reference add -c my-client --server my-server', 'my-client', 'my-server'],  # noqa: #501
    ['3_add', 'appio reference add --client my-client -s my-server', 'my-client', 'my-server'],  # noqa: #501
    ['4_add', 'appio reference add --client my-client --server my-server', 'my-client', 'my-server'],  # noqa: #501
])
def test_that_appio_reference_add_is_succeeding(host, case, command, client_app, server_app):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + client_app + ' -t Client',
        'appio new opcuaapp -n ' + server_app + ' -t Server -u 127.0.0.1 -p 4840',  # noqa: #501
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


@pytest.mark.parametrize('case, command', [
    ['1f_remove', 'appio reference remove -s my-client -c my-server'],
    ['2f_remove', 'appio reference remove -n my-client -s my-server'],
    ['3f_remove', 'appio reference remove -c my-client -n my-server'],
    ['4f_remove', 'appio reference remove -c my-client'],
    ['5f_remove', 'appio reference remove -s my-server'],
])
def test_that_appio_reference_remove_is_failing(host, case, command):
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


@pytest.mark.parametrize('case, command, client_app, server_app', [  # noqa: #501
    ['1_remove', 'appio reference remove -c my-client -s my-server', 'my-client', 'my-server'],  # noqa: #501
    ['2_remove', 'appio reference remove -c my-client --server my-server', 'my-client', 'my-server'],  # noqa: #501
    ['3_remove', 'appio reference remove --client my-client -s my-server', 'my-client', 'my-server'],  # noqa: #501
    ['4_remove', 'appio reference remove --client my-client --server my-server', 'my-client', 'my-server'],  # noqa: #501
])
def test_that_appio_reference_remove_is_succeeding(host, case, command, client_app, server_app):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + client_app + ' -t Client',
        'appio new opcuaapp -n ' + server_app + ' -t Server -u 127.0.0.1 -p 4840',  # noqa: #501
        'appio reference add -c ' + client_app + ' -s ' + server_app,
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
