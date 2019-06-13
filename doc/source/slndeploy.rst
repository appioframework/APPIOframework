.. _sln_deploy_reference:

============
sln deploy
============

Deploys all added opcuaapps of a solution into deb-packages.

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
| -s | \\--solution      |solution name           |
+------------------------+------------------------+

***************
Parent command
***************
.. parsed-literal::

   :ref:`appio_sln_reference`

********************
Detailed Description
********************

APPIO sln build executes appio deploy for each APPIO project contained in the solution.

.. note:: Before the deployment you have to execute the command :ref:`sln_publish_reference`.


