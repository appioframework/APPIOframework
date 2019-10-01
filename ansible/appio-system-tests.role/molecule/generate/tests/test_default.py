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
    ['1', 'appio generate --help'],
    ['2', 'appio generate -h'],
    ['3', 'appio generate'],
])
def test_that_appio_generate_help_is_succeeding(host, case, command):
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
    assert appio.rc == 0
    assert appio.stdout != ''

    assert_that_files_are_existing(host, file_paths)


@pytest.mark.parametrize('case, command, app_name', [
    ['1f_no_model', 'appio generate -n my-app', 'my-app'],
    ['2f_no_model', 'appio generate --name my-app', 'my-app'],
])
def test_that_appio_generate_is_failing_when_no_model_is_present(host, case, command, app_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'rm -f appio.log',
    ):
        prepare = host.run('cd ' + test_dir_path + ' && ' + prepare_command)

        assert prepare.rc == 0

    # arrange
    assert_that_files_are_missing(host, file_paths)

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc != 0
    assert appio.stdout != ''

    assert_that_files_are_existing(host, file_paths)


@pytest.mark.parametrize('case, command, app_name', [
    ['1f', 'appio generate information-model -N my-app', 'my-app'],
    ['2f', 'appio generate information-model --n my-app', 'my-app'],
    ['3f', 'appio generate information-model --Name my-app', 'my-app'],
    ['4f', 'appio generate information-model -name my-app', 'my-app'],
    ['5f', 'appio generate information-model -n my-app -m model.xml', 'my-app'],  # noqa: #501
    ['6f', 'appio generate information-model my-app', 'my-app'],
])
def test_that_appio_generate_is_failing(host, case, command, app_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'cp /test-data/information-models/minimal.xml minimal.xml',
        'appio import information-model -n ' + app_name + ' -p minimal.xml',
        'rm -f appio.log',
    ):
        prepare = host.run('cd ' + test_dir_path + ' && ' + prepare_command)

        assert prepare.rc == 0

    # arrange
    assert_that_files_are_missing(host, file_paths)

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc != 0
    assert appio.stdout != ''

    assert_that_files_are_existing(host, file_paths)


@pytest.mark.parametrize('case, command, app_name', [
    ['1', 'appio generate information-model -n my-app', 'my-app'],
    ['2', 'appio generate information-model --name my-app', 'my-app'],
])
def test_that_appio_generate_is_succeeding(host, case, command, app_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
        test_dir_path + app_name + '/src/server/information-models/minimal.c',
        test_dir_path + app_name + '/src/server/information-models/minimal.h',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'cp /test-data/information-models/minimal.xml minimal.xml',
        'appio import information-model -n ' + app_name + ' -p minimal.xml',
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
