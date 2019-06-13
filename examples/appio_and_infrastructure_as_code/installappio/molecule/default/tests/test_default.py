import os

import testinfra.utils.ansible_runner

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']).get_hosts('all')


def test_hosts_file(host):
    f = host.file('/etc/hosts')

    assert f.exists
    assert f.user == 'root'
    assert f.group == 'root'


def test_dotnet_core_is_present(host):

    package = host.package('dotnet-runtime-2.1')

    assert package.is_installed
    assert package.version.startswith('2.1')


def test_gcc_is_present(host):

    package = host.package('build-essential')
    assert package.is_installed


def test_meson_is_present(host):

    host.run_test('meson')


def test_open62541_is_present(host):

    package = host.package('open62541--v0.3.0')
    assert package.is_installed


def test_OPPO_is_present(host):

    package = host.package('oppo-terminal')
    assert package.is_installed
