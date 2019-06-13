.. _new_reference:

============
appio new
============

Creates a new APPIO project.

***************
Usage
***************

.. code-block:: bash

   appio new <argument> <options>

***************
Arguments
*************** 
+------------------------+------------------------+
| Argument               |Description             |
+========================+========================+
| opcuaapp               |will create an opcuaapp |
+------------------------+------------------------+
| sln                    |will create a solution  |      
+------------------------+------------------------+

***************
Options
***************

+------------------------+------------------------+
|   Options              |Description             |
+========================+========================+
| -h | \\--help          |prints the command help |
+------------------------+------------------------+
| -n | \\--name          |  opcuaapp/sln name     |
+------------------------+------------------------+
| -t | \\--type          |  Type of opcuapp       |
+------------------------+------------------------+
| \\--nocert             |  no certifcate creation|
+------------------------+------------------------+

+------------------------+--------------------------+
|  Types of opcuaapps    | Description              |
+========================+==========================+
| Client                 |opcuaapp will be a client |
+------------------------+--------------------------+
| Server                 |opcuaapp will be a server |
+------------------------+--------------------------+
| ClientServer           |  opcuaapp will be both   |
+------------------------+--------------------------+

+-----------------------------+------------------------+
|Options(opcuaapp server only)|Description             |
+=============================+========================+
| -u | \\--url                |  URL of the server     |
+-----------------------------+------------------------+
| -p | \\--port               |  port of the server    |
+-----------------------------+------------------------+


***************
Parent command
***************
.. parsed-literal::

   :ref:`appio_reference` 

******************** 
Detailed Description
********************

APPIO new creates a new APPIO project. There are two possible Options: creating a solution and creating an opcuaapp. 

The result is a folder with the chosen name and a basic opcuaapp/solution setup.

The solution is used for managing opcuaapps.
