============
Installation
============

Please clone the `APPIO repository <https://github.com/appioframework/APPIO-Terminal.git>`_, install all the dependencies and execute the following command at the project root:

.. code-block:: bash

   bash-developer/run-reinstall--appio-terminal--local.sh

Two packages will be installed:

-  open62541--v1.0.0
-  appio-terminal

open62541--v1.0.0 is the used OPC UA Stack.

********************
List of dependencies
********************

The dependencies of APPIO are:

-  up to date linux
-  .net Core 2.1 sdk
-  gcc compiler
-  meson
-  python3
-  dpkg

If you use Ubuntu the dependencies can be easily installed with these commands:

.. code-block:: bash

   apt update
   apt upgrade
   apt install gcc meson python3 dpkg

**********************
Using package manager
**********************

APPIO can be easily installed with the package manager.
First you need to install the open62541 stack. Continue with installing APPIO.

.. code-block:: bash

   cd /home/appiodev/Downloads/artifacts/installer/
   dpkg --install open62541--v1.0.0.deb
   dpkg --install appio-terminal.deb 

*************************
Using grahpical Installer
*************************

It is also possible to install APPIO with the graphical installer. Simply double click on the packages to install them. Keep in mind that you have to install the open62541 package first.

*************************
Testing
*************************

After installing you can test the succesfull installation with:

.. code-block:: bash

   APPIO --help

This should open the APPIO commandline help.

After that you can continue with the :ref:`tutorial_reference`.
