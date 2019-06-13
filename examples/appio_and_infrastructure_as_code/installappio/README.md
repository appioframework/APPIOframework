installappio
============

Installs APPIO on a target system. The path to the APPIO debian files has to be defined in the tasks/main.yml file.

Example Playbook
----------------

- name: Create APPIO OPC UA Example Server
  hosts: local
  roles:
    - role: installappio

License
-------

BSD

Author Information
------------------

Visit us at talsen.team.
