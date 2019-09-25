import pytest
import os

import testinfra.utils.ansible_runner

from .util.prepare import prepare_provide_test_directory

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']
).get_hosts('all')


@pytest.mark.parametrize('case, command, app_name', [
    ['1', 'appio deploy --name my-app', 'my-app'],
    ['2', 'appio deploy -n     my-app', 'my-app'],
])
def test_that_appio_deploy_is_succeeding(host, case, command, app_name):
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    deb_file_path = test_dir_path + app_name + '/deploy/appio-opcuaapp.deb'

    file_paths = [
        test_dir_path + 'appio.log',
        deb_file_path
    ]

    installed_file_paths = [
        '/usr/bin/client-app',
        '/usr/bin/server-app',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'appio build -n ' + app_name,
        'appio publish -n ' + app_name,
        'rm -f appio.log',
    ):
        prepare = host.run('cd ' + test_dir_path + ' && ' + prepare_command)

        assert prepare.rc == 0

    # arrange
    for file_path in file_paths:
        f = host.file(file_path)
        assert not f.exists

    for file_path in installed_file_paths:
        f = host.file(file_path)
        assert not f.exists

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc == 0
    assert appio.stdout != ''

    for file_path in file_paths:
        f = host.file(file_path)
        assert f.exists

    # act installer
    dpkg_install = host.run('dpkg --install ' + deb_file_path)

    # assert installer
    assert dpkg_install.rc == 0

    for file_path in installed_file_paths:
        f = host.file(file_path)
        assert f.exists
        assert f.mode == 0o755

    # cleanup
    dpkg_purge = host.run('dpkg --purge appio-opcuaapp-installer')
    assert dpkg_purge.rc == 0


@pytest.mark.parametrize('case, command', [
    ['1f', 'appio deploy --name my-app-5263452364'],
    ['2f', 'appio deploy -n     my-app-5263452364'],
    ['3f', 'appio deploy --name my/-app'],
    ['4f', 'appio deploy -n     my/-app'],
    ['5f', 'appio deploy --name'],
    ['6f', 'appio deploy -n'],
    ['7f', 'appio deploy --exit'],
    ['8f', 'appio deploy -x'],
])
def test_that_appio_deploy_is_failing(host, case, command):
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    # arrange
    for file_path in file_paths:
        f = host.file(file_path)
        assert not f.exists

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc != 0
    assert appio.stdout != ''

    for file_path in file_paths:
        f = host.file(file_path)
        assert f.exists


@pytest.mark.parametrize('case, command, app_name', [
    ['1', 'appio deploy --name my-app', 'my-app'],
    ['2', 'appio deploy -n     my-app', 'my-app'],
])
def test_that_appio_deploy_is_failing_when_published_files_are_missing(host, case, command, app_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    missing_file_paths = [
        test_dir_path + app_name + '/deploy/appio-opcuaapp.deb',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'appio build -n ' + app_name,
        'appio publish -n ' + app_name,
        file_paths[0],
        test_dir_path + 'rm -f ' + app_name + '/publish/client-app',
        test_dir_path + 'rm -f ' + app_name + '/publish/server-app',
    ):
        prepare = host.run('cd ' + test_dir_path + ' && ' + prepare_command)

        assert prepare.rc == 0

    # arrange
    for file_path in file_paths:
        f = host.file(file_path)
        assert not f.exists

    for file_path in missing_file_paths:
        f = host.file(file_path)
        assert not f.exists

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc != 0
    assert appio.stdout != ''

    for file_path in file_paths:
        f = host.file(file_path)
        assert f.exists

    for file_path in missing_file_paths:
        f = host.file(file_path)
        assert not f.exists
