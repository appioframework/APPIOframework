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
    ['1_help', 'appio sln --help'],
    ['2_help', 'appio sln -h'],
    ['3_help', 'appio sln'],
])
def test_that_appio_sln_help_is_succeeding(host, case, command):
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
    ['1f_add', 'appio sln add -n my-sln -p my-app'],
    ['2f_add', 'appio sln add -s my-sln -n my-app'],
    ['3f_add', 'appio sln add -p my-sln -s my-app'],
    ['4f_add', 'appio sln add -s my-sln'],
    ['5f_add', 'appio sln add -p my-app'],
])
def test_that_appio_sln_add_is_failing(host, case, command):
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


@pytest.mark.parametrize('case, command, app_name, sln_name', [
    ['1_add', 'appio sln add -s my-sln -p my-app', 'my-app', 'my-sln'],  # noqa: #501
    ['2_add', 'appio sln add -s my-sln --project my-app', 'my-app', 'my-sln'],  # noqa: #501
    ['3_add', 'appio sln add --solution my-sln -p my-app', 'my-app', 'my-sln'],  # noqa: #501
    ['4_add', 'appio sln add --solution my-sln --project my-app', 'my-app', 'my-sln'],  # noqa: #501
])
def test_that_appio_sln_add_is_succeeding(host, case, command, app_name, sln_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'appio new sln -n ' + sln_name,
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
    ['1f_remove', 'appio sln remove -n my-sln -p my-app'],
    ['2f_remove', 'appio sln remove -s my-sln -n my-app'],
    ['3f_remove', 'appio sln remove -p my-sln -s my-app'],
    ['4f_remove', 'appio sln remove -s my-sln'],
    ['5f_remove', 'appio sln remove -p my-app'],
])
def test_that_appio_sln_remove_is_failing(host, case, command):
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


@pytest.mark.parametrize('case, command, app_name, sln_name', [
    ['1_remove', 'appio sln remove -s my-sln -p my-app', 'my-app', 'my-sln'],  # noqa: #501
    ['2_remove', 'appio sln remove -s my-sln --project my-app', 'my-app', 'my-sln'],  # noqa: #501
    ['3_remove', 'appio sln remove --solution my-sln -p my-app', 'my-app', 'my-sln'],  # noqa: #501
    ['4_remove', 'appio sln remove --solution my-sln --project my-app', 'my-app', 'my-sln'],  # noqa: #501
])
def test_that_appio_sln_remove_is_succeeding(host, case, command, app_name, sln_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'appio new sln -n ' + sln_name,
        'appio sln add -s ' + sln_name + ' -p ' + app_name,
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
    ['1f_build', 'appio sln build -n my-sln'],
    ['2f_build', 'appio sln build --name my-sln'],
    ['3f_build', 'appio sln build -s'],
    ['4f_build', 'appio sln build --solution'],
])
def test_that_appio_sln_build_is_failing(host, case, command):
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


@pytest.mark.parametrize('case, command, app_name, sln_name', [
    ['1_build', 'appio sln build -s my-sln', 'my-app', 'my-sln'],  # noqa: #501
    ['2_build', 'appio sln build --solution my-sln', 'my-app', 'my-sln'],  # noqa: #501
])
def test_that_appio_sln_build_is_succeeding(host, case, command, app_name, sln_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
        test_dir_path + app_name + '/build/client-app',
        test_dir_path + app_name + '/build/server-app',
    ]

    exe_file_paths = [
        test_dir_path + app_name + '/build/client-app',
        test_dir_path + app_name + '/build/server-app',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'appio new sln -n ' + sln_name,
        'appio sln add -s ' + sln_name + ' -p ' + app_name,
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
    ['1f_publish', 'appio sln publish -n my-sln'],
    ['2f_publish', 'appio sln publish --name my-sln'],
    ['3f_publish', 'appio sln publish -s'],
    ['4f_publish', 'appio sln publish --solution'],
])
def test_that_appio_sln_publish_is_failing(host, case, command):
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


@pytest.mark.parametrize('case, command, app_name, sln_name', [
    ['1_publish', 'appio sln publish -s my-sln', 'my-app', 'my-sln'],  # noqa: #501
    ['2_publish', 'appio sln publish --solution my-sln', 'my-app', 'my-sln'],  # noqa: #501
])
def test_that_appio_sln_publish_is_succeeding(host, case, command, app_name, sln_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
        test_dir_path + app_name + '/publish/client-app',
        test_dir_path + app_name + '/publish/server-app',
    ]

    exe_file_paths = [
        test_dir_path + app_name + '/publish/client-app',
        test_dir_path + app_name + '/publish/server-app',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'appio new sln -n ' + sln_name,
        'appio sln add -s ' + sln_name + ' -p ' + app_name,
        'appio sln build -s ' + sln_name,
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
    ['1f_deploy', 'appio sln deploy -n my-sln'],
    ['2f_deploy', 'appio sln deploy --name my-sln'],
    ['3f_deploy', 'appio sln deploy -s'],
    ['4f_deploy', 'appio sln deploy --solution'],
])
def test_that_appio_sln_deploy_is_failing(host, case, command):
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


@pytest.mark.parametrize('case, command, app_name, sln_name', [
    ['1_deploy', 'appio sln deploy -s my-sln', 'my-app', 'my-sln'],  # noqa: #501
    ['2_deploy', 'appio sln deploy --solution my-sln', 'my-app', 'my-sln'],  # noqa: #501
])
def test_that_appio_sln_deploy_is_succeeding(host, case, command, app_name, sln_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    deb_file_path = test_dir_path + app_name + '/deploy/appio-opcuaapp.deb'

    file_paths = [
        test_dir_path + 'appio.log',
        deb_file_path,
    ]

    installed_file_paths = [
        '/usr/bin/client-app',
        '/usr/bin/server-app',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'appio new sln -n ' + sln_name,
        'appio sln add -s ' + sln_name + ' -p ' + app_name,
        'appio sln build -s ' + sln_name,
        'appio sln publish -s ' + sln_name,
        'rm -f appio.log',
    ):
        prepare = host.run('cd ' + test_dir_path + ' && ' + prepare_command)

        assert prepare.rc == 0

    # arrange
    assert_that_files_are_missing(host, file_paths)
    assert_that_files_are_missing(host, installed_file_paths)

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc == 0
    assert appio.stdout != ''

    assert_that_files_are_existing(host, file_paths)

    # act installer
    dpkg_install = host.run('dpkg --install ' + deb_file_path)

    # assert installer
    assert dpkg_install.rc == 0

    assert_that_files_are_existing(host, installed_file_paths)
    assert_that_files_are_executable(host, installed_file_paths)

    # cleanup
    dpkg_purge = host.run('dpkg --purge appio-opcuaapp-installer')

    assert dpkg_purge.rc == 0
