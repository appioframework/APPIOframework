.. _generate_reference:

==============
appio generate
==============

Generate informationmodels or certificates in the APPIO project.

***************
Usage
***************

.. code-block:: bash

   appio generate <argument> <options>

***************
Arguments
*************** 
+------------------------+-----------------------------+
| Argument               |Description                  |
+========================+=============================+
| information-model      |Generate an informationmodel |
+------------------------+-----------------------------+
| certificate            |Generate a certificate       |
+------------------------+-----------------------------+      

***************
Options
***************
+------------------------+------------------------+
|   Options              |Description             |
+========================+========================+
| -h | \\--help          |prints the command help |
+------------------------+------------------------+
| -n | \\--name          |  appio project name    |
+------------------------+------------------------+

+-------------------------+---------------------------------------------------------+
|Options(certificate only)|Description                                              |
+=========================+=========================================================+
| \\--days                |duration of the certificate in days(default: 365 days)   |
+-------------------------+---------------------------------------------------------+
| \\--keysize             |size of the key(default : 1024 bit)                      |
+-------------------------+---------------------------------------------------------+
| \\--organization        |Organization name used for certification (default: MyOrg)|
+-------------------------+---------------------------------------------------------+

***************
Parent command
***************
.. parsed-literal::

   :ref:`appio_reference`

********************
Detailed Description
********************

Generates either an OPC UA certificate or the source code of the imported information-model.  

After the generation of the informationmodel, :ref:`build_reference` needs to be called. 

.. note:: You have to import an informationmodel with :ref:`import_reference`.
