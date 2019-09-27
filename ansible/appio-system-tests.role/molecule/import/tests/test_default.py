import pytest
import os

import testinfra.utils.ansible_runner

from .util.prepare import assert_that_files_are_existing
from .util.prepare import assert_that_files_are_missing
from .util.prepare import prepare_provide_test_directory

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']
).get_hosts('all')


@pytest.mark.parametrize('case, command, app_name', [
    ['1f', 'appio import -n my-app -p model.xml', 'my-app'],
    ['2f', 'appio import -n my-app --path model.xml', 'my-app'],
    ['3f', 'appio import --name my-app -p model.xml', 'my-app'],
    ['4f', 'appio import --name --path model.xml', 'my-app'],
    ['5f', 'appio import -n my-app -s', 'my-app'],
    ['6f', 'appio import -n my-app --sample', 'my-app'],
    ['7f', 'appio import --name my-app -s', 'my-app'],
    ['8f', 'appio import --name my-app --sample', 'my-app'],
])
def test_that_appio_import_is_failing(host, case, command, app_name):
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
    ['1f', 'appio import information-model -n my-app -p model.txt', 'my-app'],  # noqa: #501
    ['2f', 'appio import information-model -n my-app --path model.xml2', 'my-app'],  # noqa: #501
    ['3f', 'appio import information-model -n my-app --Path model.xml2', 'my-app'],  # noqa: #501
    ['4f', 'appio import information-model -n my-app -P model.xml2', 'my-app'],  # noqa: #501
    ['5f', 'appio import information-model -n my-app -p notExistingModel.xml', 'my-app'],  # noqa: #501
    ['6f', 'appio import information-model -nn my-app -p notExistingModel.xml', 'my-app'],  # noqa: #501
    ['7f', 'appio import information-model my-app -p notExistingModel.xml', 'my-app'],  # noqa: #501
])
def test_that_appio_import_information_model_is_failing(host, case, command, app_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'touch model.xml',
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
