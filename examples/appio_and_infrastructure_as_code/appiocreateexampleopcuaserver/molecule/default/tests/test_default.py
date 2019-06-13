import os

import testinfra.utils.ansible_runner

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']).get_hosts('all')


def test_hosts_file(host):
    f = host.file('/etc/hosts')

    assert f.exists
    assert f.user == 'root'
    assert f.group == 'root'


def test_oppo_folder_was_created(host):

    opcuaapp = host.file('oppo/')

    assert opcuaapp.exists


def test_oppo_opcuaapp_was_created(host):

    opcuaapp = host.file('oppo/oppotutorial')

    assert opcuaapp.exists


def test_oppo_opcuaapp_was_builded(host):

    opcuaapp = host.file('oppo/oppotutorial/build')

    assert opcuaapp.exists


def test_oppo_opcuaapp_was_published(host):

    opcuaapp = host.file('oppo/oppotutorial/publish')

    assert opcuaapp.exists


def test_oppo_opcuaapp_was_deployed(host):

    opcuaapp = host.file('oppo/oppotutorial/deploy')

    assert opcuaapp.exists
