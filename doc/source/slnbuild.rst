.. _sln_build_reference:

============
sln build
============

Builds an APPIO solution, which means all added opcuaapps are built.

***************
Usage
***************

.. code-block:: bash

   appio sln build <options>

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

***************
Parent command
***************
.. parsed-literal::

   :ref:`appio_sln_reference`

********************
Detailed Description
********************

APPIO sln build executes appio build for each APPIO project contained in the solution.


.. note:: After a successfull build you can publish the opcuaapp with appio :ref:`sln_publish_reference`.
