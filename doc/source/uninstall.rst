==============
Uninstallation
==============

********************
With package manager
********************

To uninstall APPIO simply execute the following commands:

.. code-block:: bash

   dpkg -r appio-terminal
   dpkg -r open62541--v0.3.0

*************************
Testing
*************************

After installing you can test the succesfull uninstallation with:

.. code-block:: bash

   appio --help

The APPIO help should not be displayed.