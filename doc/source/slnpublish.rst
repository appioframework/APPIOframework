.. _sln_publish_reference:

============
sln publish
============

Prepares the added opcuaapps for usage.

***************
Usage
***************

.. code-block:: bash

   appio sln publish <options>

***************
Options
***************
+------------------------+------------------------+
|   Options              |Description             |
+========================+========================+
| -h | \\--help          |prints the command help |
+------------------------+------------------------+
| -s | \\--solution      |  appio solution name   |
+------------------------+------------------------+

***************
Parent command
***************
.. parsed-literal::

   :ref:`appio_sln_reference`

********************
Detailed Description
********************

APPIO sln publish executes appio publish for each APPIO project contained in the solution.


.. note:: You can use the :ref:`deploy_reference` command to deploy the opcuaapps into debian packages.

.. note:: There is no need to deploy the opcuaapps. You can use the binaries right away.
