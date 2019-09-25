import pytest
import os

import testinfra.utils.ansible_runner

from .util.prepare import prepare_provide_test_directory

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']
).get_hosts('all')


@pytest.mark.parametrize('case, command', [
    ['1_help', 'appio build --help'],
    ['2_help', 'appio build -h'],
    ['3_help', 'appio build'],
])
def test_that_appio_build_help_is_succeeding(host, case, command):
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log'
    ]

    # arrange
    for file_path in file_paths:
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


@pytest.mark.parametrize('case, command, app_name', [
    ['1', 'appio build --name my-app', 'my-app'],
    ['2', 'appio build -n     my-app', 'my-app'],
])
def test_that_appio_build_opcuaapp_is_succeeding(host, case, command, app_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
        test_dir_path + app_name + '/build/client-app',
        test_dir_path + app_name + '/build/server-app',
    ]

    exe_file_paths = [
        file_paths[1],
        file_paths[2],
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'rm -f appio.log',
    ):
        prepare = host.run('cd ' + test_dir_path + ' && ' + prepare_command)

        assert prepare.rc == 0

    # arrange
    for file_path in file_paths:
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

    for file_path in exe_file_paths:
        f = host.file(file_path)

        assert f.mode == 0o755


@pytest.mark.parametrize('case, command', [
    ['1f', 'appio build --name my-app-5263452364'],
    ['2f', 'appio build -n     my-app-5263452364'],
    ['3f', 'appio build --name my/-app'],
    ['4f', 'appio build -n     my/-app'],
    ['5f', 'appio build --name'],
    ['6f', 'appio build -n'],
    ['7f', 'appio build --exit'],
    ['8f', 'appio build -x'],
])
def test_that_appio_build_opcuaapp_is_failing(host, case, command):
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
    ['1f_meson', 'appio build --name my-app', 'my-app'],
])
def test_that_appio_build_opcuaapp_is_failing_when_meson_call_fails(host, case, command, app_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'rm -f ' + app_name + '/meson.build',
        'rm -f appio.log',
    ):
        prepare = host.run('cd ' + test_dir_path + ' && ' + prepare_command)

        assert prepare.rc == 0

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
    ['1f_ninja', 'appio build --name my-app', 'my-app'],
])
def test_that_appio_build_opcuaapp_is_failing_when_ninja_call_fails(host, case, command, app_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        test_dir_path + 'appio.log',
    ]

    for prepare_command in (
        'appio new opcuaapp -n ' + app_name + ' -t ClientServer -u 127.0.0.1 -p 4840',  # noqa: #501
        'rm -f ' + app_name + '/src/server/main.c',
        'rm -f appio.log',
    ):
        prepare = host.run('cd ' + test_dir_path + ' && ' + prepare_command)

        assert prepare.rc == 0

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
