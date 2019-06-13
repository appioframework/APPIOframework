.. _sln_add_reference:

============
sln add
============

Adds an opcuaapp to an APPIO solution.

***************
Usage
***************

.. code-block:: bash

   appio sln add <options>

***************
Options
***************
+------------------------+------------------------+
|   Options              |Description             |
+========================+========================+
| -h | \\--help          |prints the command help |
+------------------------+------------------------+
| -s | \\--solution      |  APPIO solution name   |
+------------------------+------------------------+
| -p | \\--project       |  opcuaapp name         |
+------------------------+------------------------+

***************
Parent command
***************
.. parsed-literal::

   :ref:`appio_sln_reference`

********************
Detailed Description
********************

Sln add adds a chosen opcuaapp to the defined APPIO solution.   

.. note:: If you use multiple opcuaapps, a solution makes the handling of these much easier.
   Example: You can build all the added opcuaapps with appio :ref:`sln_build_reference`.
