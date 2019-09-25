import pytest
import os

import testinfra.utils.ansible_runner

from .util.prepare import prepare_provide_test_directory

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']
).get_hosts('all')


@pytest.mark.parametrize('case, command, app_name', [
    ['1_client', 'appio new opcuaapp -n my-app -t Client', 'my-app'],
    ['2_client', 'appio new opcuaapp -n my-app --type Client', 'my-app'],
    ['3_client', 'appio new opcuaapp --name my-app -t Client', 'my-app'],
    ['4_client', 'appio new opcuaapp --name my-app --type Client', 'my-app'],
])
def test_that_appio_new_opcuaapp_client_is_succeeding(host, case, command, app_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    priv_der_file_path = test_dir_path + app_name + '/certificates/priv.der'  # noqa: #501
    cert_der_file_path = test_dir_path + app_name + '/certificates/cert.der'  # noqa: #501

    file_paths = [
        priv_der_file_path,
        cert_der_file_path,
        test_dir_path + 'appio.log',
        test_dir_path + app_name + '/' + app_name + '.appioproj',
        test_dir_path + app_name + '/meson.build',
        test_dir_path + app_name + '/src/client/main.c',
        test_dir_path + app_name + '/src/client/globalVariables.h',
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

    openssl_priv_der = host.run('openssl  rsa -in ' + priv_der_file_path + ' -inform der -noout')  # noqa: #501
    assert openssl_priv_der.rc == 0
    openssl_cert_der = host.run('openssl x509 -in ' + cert_der_file_path + ' -inform der -noout')  # noqa: #501
    assert openssl_cert_der.rc == 0


@pytest.mark.parametrize('case, command, app_name', [
    ['1_server', 'appio new opcuaapp -n my-app -t Server -u 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['2_server', 'appio new opcuaapp -n my-app -t Server -u 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['3_server', 'appio new opcuaapp -n my-app -t Server --url 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['4_server', 'appio new opcuaapp -n my-app -t Server --url 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['5_server', 'appio new opcuaapp -n my-app --type Server -u 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['6_server', 'appio new opcuaapp -n my-app --type Server -u 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['7_server', 'appio new opcuaapp -n my-app --type Server --url 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['8_server', 'appio new opcuaapp -n my-app --type Server --url 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['9_server', 'appio new opcuaapp --name my-app -t Server -u 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['10_server', 'appio new opcuaapp --name my-app -t Server -u 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['11_server', 'appio new opcuaapp --name my-app -t Server --url 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['12_server', 'appio new opcuaapp --name my-app -t Server --url 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['13_server', 'appio new opcuaapp --name my-app --type Server -u 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['14_server', 'appio new opcuaapp --name my-app --type Server -u 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['15_server', 'appio new opcuaapp --name my-app --type Server --url 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['16_server', 'appio new opcuaapp --name my-app --type Server --url 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
])
def test_that_appio_new_opcuaapp_server_is_succeeding(host, case, command, app_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    priv_der_file_path = test_dir_path + app_name + '/certificates/priv.der'  # noqa: #501
    cert_der_file_path = test_dir_path + app_name + '/certificates/cert.der'  # noqa: #501

    dir_paths = [
        test_dir_path + app_name + '/models',
    ]

    file_paths = [
        priv_der_file_path,
        cert_der_file_path,
        test_dir_path + 'appio.log',
        test_dir_path + app_name + '/' + app_name + '.appioproj',
        test_dir_path + app_name + '/meson.build',
        test_dir_path + app_name + '/src/server/main.c',
        test_dir_path + app_name + '/src/server/meson.build',
        test_dir_path + app_name + '/src/server/loadInformationModels.c',
        test_dir_path + app_name + '/src/server/constants.h',
        test_dir_path + app_name + '/src/server/mainCallbacks.c',
    ]

    # arrange
    for file_path in file_paths:
        f = host.file(file_path)
        assert not f.exists

    for dir_path in dir_paths:
        f = host.file(dir_path)
        assert not f.exists

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc == 0
    assert appio.stdout != ''

    for file_path in file_paths:
        f = host.file(file_path)
        assert f.exists

    for dir_path in dir_paths:
        f = host.file(dir_path)
        assert f.exists
        assert f.is_directory

    openssl_priv_der = host.run('openssl  rsa -in ' + app_name + '/certificates/priv.der -inform der -noout')  # noqa: #501
    assert openssl_priv_der.rc == 0
    openssl_cert_der = host.run('openssl x509 -in ' + app_name + '/certificates/cert.der -inform der -noout')  # noqa: #501
    assert openssl_cert_der.rc == 0


@pytest.mark.parametrize('case, command, app_name', [
    ['1_client_server', 'appio new opcuaapp -n my-app -t ClientServer -u 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['2_client_server', 'appio new opcuaapp -n my-app -t ClientServer -u 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['3_client_server', 'appio new opcuaapp -n my-app -t ClientServer --url 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['4_client_server', 'appio new opcuaapp -n my-app -t ClientServer --url 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['5_client_server', 'appio new opcuaapp -n my-app --type ClientServer -u 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['6_client_server', 'appio new opcuaapp -n my-app --type ClientServer -u 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['7_client_server', 'appio new opcuaapp -n my-app --type ClientServer --url 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['8_client_server', 'appio new opcuaapp -n my-app --type ClientServer --url 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['9_client_server', 'appio new opcuaapp --name my-app -t ClientServer -u 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['10_client_server', 'appio new opcuaapp --name my-app -t ClientServer -u 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['11_client_server', 'appio new opcuaapp --name my-app -t ClientServer --url 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['12_client_server', 'appio new opcuaapp --name my-app -t ClientServer --url 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['13_client_server', 'appio new opcuaapp --name my-app --type ClientServer -u 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['14_client_server', 'appio new opcuaapp --name my-app --type ClientServer -u 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
    ['15_client_server', 'appio new opcuaapp --name my-app --type ClientServer --url 127.0.0.1 -p 4840', 'my-app'],  # noqa: #501
    ['16_client_server', 'appio new opcuaapp --name my-app --type ClientServer --url 127.0.0.1 --port 4840', 'my-app'],  # noqa: #501
])
def test_that_appio_new_opcuaapp_clientserver_is_succeeding(host, case, command, app_name):  # noqa: #501
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    client_priv_der_file_path = test_dir_path + app_name + '/certificates/client_priv.der'  # noqa: #501
    client_cert_der_file_path = test_dir_path + app_name + '/certificates/client_cert.der'  # noqa: #501

    server_priv_der_file_path = test_dir_path + app_name + '/certificates/server_priv.der'  # noqa: #501
    server_cert_der_file_path = test_dir_path + app_name + '/certificates/server_cert.der'  # noqa: #501

    dir_paths = [
        test_dir_path + app_name + '/models',
    ]

    file_paths = [
        client_priv_der_file_path,
        client_cert_der_file_path,
        server_priv_der_file_path,
        server_cert_der_file_path,
        test_dir_path + 'appio.log',
        test_dir_path + app_name + '/' + app_name + '.appioproj',
        test_dir_path + app_name + '/meson.build',
        test_dir_path + app_name + '/src/client/main.c',
        test_dir_path + app_name + '/src/client/globalVariables.h',
        test_dir_path + app_name + '/src/server/main.c',
        test_dir_path + app_name + '/src/server/meson.build',
        test_dir_path + app_name + '/src/server/loadInformationModels.c',
        test_dir_path + app_name + '/src/server/constants.h',
        test_dir_path + app_name + '/src/server/mainCallbacks.c',
    ]

    # arrange
    for file_path in file_paths:
        f = host.file(file_path)
        assert not f.exists

    for dir_path in dir_paths:
        f = host.file(dir_path)
        assert not f.exists

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc == 0
    assert appio.stdout != ''

    for file_path in file_paths:
        f = host.file(file_path)
        assert f.exists

    for dir_path in dir_paths:
        f = host.file(dir_path)
        assert f.exists
        assert f.is_directory

    openssl_priv_der = host.run('openssl  rsa -in ' + app_name + '/certificates/priv.der -inform der -noout')  # noqa: #501
    assert openssl_priv_der.rc == 0
    openssl_cert_der = host.run('openssl x509 -in ' + app_name + '/certificates/cert.der -inform der -noout')  # noqa: #501
    assert openssl_cert_der.rc == 0


@pytest.mark.parametrize('case, command', [
    ['1f', 'appio new opcuaapp --name test/-solution'],
    ['2f', 'appio new opcuaapp -n     test/-solution'],
    ['3f', 'appio new opcuaapp --name'],
    ['4f', 'appio new opcuaapp -n'],
    ['5f', 'appio new opcuaapp --exit'],
    ['6f', 'appio new opcuaapp -x'],
    ['7f', 'appio new opcuaapp -n my-app -type Client'],
    ['8f', 'appio new opcuaapp -n my-app --t Server'],
    ['9f', 'appio new opcuaapp -p my-app -t ClientServer'],
    ['10f', 'appio new opcuaapp -n my-app -t'],
    ['11f', 'appio new opcuaapp -n my-app --type'],
    ['12f', 'appio new opcuaapp -n my-app'],
    ['13f', 'appio new opcuaapp --name my-app'],
])
def test_that_appio_new_opcuaapp_is_failing(host, case, command):
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
    assert appio.rc != 0
    assert appio.stdout != ''

    for file_path in file_paths:
        f = host.file(file_path)
        assert f.exists
