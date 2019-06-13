.. _build_reference:

============
appio build
============

Builds an opcuaapp from an APPIO project

***************
Usage
***************

.. code-block:: bash

   appio build <options>

***************
Options
***************
+------------------------+------------------------+
|   Options              |Description             |
+========================+========================+
| -h | \\--help          |prints the command help |
+------------------------+------------------------+
| -n | \\--name          |  APPIO project name    |
+------------------------+------------------------+

***************
Parent command
***************
.. parsed-literal::

   :ref:`appio_reference`

********************
Detailed Description
********************

APPIO build buildes a chosen APPIO project, including all generated information-models and the user specific application code. The binaries are created in the build folder inside of the APPIO project. For building, Meson and Ninja are used.  

.. note:: After a successfull build you can publish the opcuaapp with :ref:`publish_reference`. This copies the binaries into the publish folder for further use.
