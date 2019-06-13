.. _import_reference:

============
appio import
============

Imports external resources.

***************
Usage
***************

.. code-block:: bash

   appio import <argument> <options>

***************
Arguments
*************** 
+------------------------+-----------------------------+
| Argument               |Description                  |
+========================+=============================+
| information-model      |import an informationmodel   |
+------------------------+-----------------------------+
| certificate            |import a certificate         |
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

+-------------------------+--------------------------------------------------------------+
|Options(certificate only)|Description                                                   |
+=========================+==============================================================+
| -c | \\--certificate    |path to the certificate                                       |
+-------------------------+--------------------------------------------------------------+
| \\--client or \\--server|if the APPIO project is both a server and a client, this is   |
|                         |used to determine in which the certificate should be imported |
+-------------------------+--------------------------------------------------------------+
| -k | \\--key            |path to the keyfile                                           |
+-------------------------+--------------------------------------------------------------+

+------------------------------+---------------------------------------------------------+
|Options(informationmodel only)|Description                                              |
+==============================+=========================================================+
| -p | \\--path                |path of the information model certificate                |
+------------------------------+---------------------------------------------------------+
| -s | \\--sample              |choose a sample provided by APPIO                        |
+------------------------------+---------------------------------------------------------+




***************
Parent command
***************
.. parsed-literal::

   :ref:`appio_reference`

********************
Detailed Description
********************

APPIO import imports an OPC UA conform information model or an existing X509 certificate and and corresponding private key.S 

Alternatively a sample information model provided by APPIO can be used. The certificate can also be generated with appio :ref:`generate_reference`.

Available samples are: DI-Informationmodel.

.. note:: The imported information-model needs to generated with :ref:`generate_reference`.
