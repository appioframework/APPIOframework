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
    ['1', 'appio deploy --name my-app', 'my-app'],
    ['2', 'appio deploy -n     my-app', 'my-app'],
])
def test_that_deploy_clean_is_succeeding(host, case, command, app_name):
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    log_file_path = test_dir_path + 'appio.log'
    deb_file_path = test_dir_path + app_name + 'deploy/appio-opcuaapp.deb'
    client_app_exe_path = '/usr/bin/client-app'
    server_app_exe_path = '/usr/bin/server-app'

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'appio build -n ' + app_name,
        'appio publish -n ' + app_name,
        'rm -f appio.log',
    ):
        prepare = host.run('cd ' + test_dir_path + ' && ' + prepare_command)

        assert prepare.rc == 0

    # arrange
    log_file = host.file(log_file_path)
    assert not log_file.exists

    client_app_exe = host.file(client_app_exe_path)
    assert not client_app_exe.exists

    server_app_exe = host.file(server_app_exe_path)
    assert not server_app_exe.exists

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc == 0
    assert appio.stdout != ''

    log_file = host.file(log_file_path)
    assert log_file.exists

    deb_file = host.file(deb_file_path)
    assert deb_file.exists

    # act installer
    dpkg_install = host.run('dpkg --install ' + deb_file_path)

    # assert installer
    assert dpkg_install.rc == 0

    client_app_exe = host.file(client_app_exe_path)
    assert client_app_exe.exists

    server_app_exe = host.file(server_app_exe_path)
    assert server_app_exe.exists

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


@pytest.mark.parametrize('case, command, app_name', [
    ['1', 'appio deploy --name my-app', 'my-app'],
    ['2', 'appio deploy -n     my-app', 'my-app'],
])
def test_that_deploy_clean_is_failing_when_published_files_are_missing(host, case, command, app_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    log_file_path = test_dir_path + 'appio.log'
    deb_file_path = test_dir_path + app_name + 'deploy/appio-opcuaapp.deb'  # noqa: #501

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'appio build -n ' + app_name,
        'appio publish -n ' + app_name,
        'rm -f appio.log',
        'rm -f ' + app_name + '/publish/client-app',
        'rm -f ' + app_name + '/publish/server-app',
    ):
        prepare = host.run('cd ' + test_dir_path + ' && ' + prepare_command)

        assert prepare.rc == 0

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
