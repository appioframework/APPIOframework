.. _reference_reference:

===============
appio reference
===============

Add or remove a reference of a server to/from an Client. 

***************
Usage
***************

.. code-block:: bash

   appio reference <argument> <options>

***************
Arguments
*************** 
+------------------------+-----------------------------+
| Argument               |Description                  |
+========================+=============================+
| add                    |add an OPCU UA server        |
+------------------------+-----------------------------+
| remove                 |remove an OPCU UA server     |
+------------------------+-----------------------------+      

***************
Options
***************
+------------------------+------------------------+
|   Options              |Description             |
+========================+========================+
| -c | \\--client        |target client           |
+------------------------+------------------------+
| -s | \\--server        |  target server         |
+------------------------+------------------------+
| -h | \\--help          | opens the command help |
+------------------------+------------------------+


***************
Parent command
***************
.. parsed-literal::

   :ref:`appio_reference`

********************
Detailed Description
********************

Add or remove a reference of an APPIO OPC UA server to/from an APPIO OPC UA Client. 

.. note:: If you want to access the data nodes from the target server, check out the source code from this example :ref:`smartpump_reference`.