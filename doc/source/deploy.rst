.. _deploy_reference:

============
appio deploy
============

Create a deb-package containing the opcuaapp.

***************
Usage
***************

.. code-block:: bash

   appio deploy <options>

***************
Options
***************
+------------------------+------------------------+
|   Options              |Description             |
+========================+========================+
| -h | \\--help          |prints the command help |
+------------------------+------------------------+
| -n | \\--name          |appio project name      |
+------------------------+------------------------+

***************
Parent command
***************
.. parsed-literal::

   :ref:`appio_reference`

********************
Detailed Description
********************

This wraps the opcuaapp into a debian package with the name 'appio-opcuaapp.deb' inside the now generated subdirectory 'deploy'.


.. note:: Before the deployment you have to execute the command :ref:`publish_reference`.


