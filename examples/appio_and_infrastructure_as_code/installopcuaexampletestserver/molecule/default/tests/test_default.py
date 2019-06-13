import os

import testinfra.utils.ansible_runner

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']).get_hosts('all')


def test_opcuaappdebfile_is_present(host):
    debfile = host.file('oppo-opcuaapp.deb')

    assert debfile.exists


def test_opcuaapp_is_installed(host):

    package = host.package('oppo-opcuaapp-installer')
    assert package.is_installed
