import pytest
import os

import testinfra.utils.ansible_runner

from ....python.prepare import prepare_provide_test_directory

testinfra_hosts = testinfra.utils.ansible_runner.AnsibleRunner(
    os.environ['MOLECULE_INVENTORY_FILE']
).get_hosts('all')


@pytest.mark.parametrize('case, command', [
    ['1', 'appio build --help'],
    ['2', 'appio build -h'],
    ['3', 'appio build'],
])
def test_that_appio_build_help_is_succeeding(host, case, command):
    # prepare
    test_dir_path = prepare_provide_test_directory(host, case)

    file_paths = [
        'appio.log'
    ]

    # arrange
    for file_path in file_paths:
        f = host.file(test_dir_path + file_path)
        assert not f.exists

    # act
    appio = host.run('cd ' + test_dir_path + ' && ' + command)

    # assert
    assert appio.rc == 0
    assert appio.stdout != ''

    for file_path in file_paths:
        f = host.file(test_dir_path + file_path)
        assert f.exists
