# APPIO framework

## Adaptable Plug & Produce Industrial Orchestration Framework

<a href="http://apppioframework.org" target="_blank">
<img style="float: right;" src="logo.png">
</a>

**APPIO is a command-line utility for managing an OPC UA infrastructure. It is built on [open62541](https://github.com/open62541/open62541/), an open-source implementation of OPC UA, and it is published under the Mozilla Public License.**

[![Travis (.org) master](https://img.shields.io/travis/appioframework/APPIOframework/master?style=for-the-badge)](//travis-ci.org/appioframework/APPIOframework)
[![Travis (.org)](https://img.shields.io/travis/appioframework/APPIOframework?style=for-the-badge)](//travis-ci.org/appioframework/APPIOframework)
[![Read the Docs](https://img.shields.io/readthedocs/appioframework.svg?style=for-the-badge)](//appioframework.readthedocs.io/en/latest/)
[![GitHub](https://img.shields.io/github/license/appioframework/APPIOframework.svg?style=for-the-badge)](LICENCE)

## Dependencies

### Development dependencies

To develop on APPIO the following modules must be installed:  

- ansible
- docker
- dotnet core sdk 2.1
- molecule

To build its debian installer on your local system following modules must be installed:  

- docker

### Runtime dependencies

To install and use APPIO the following modules must be installed:  

- dotnet core runtime 2.1
- gcc
- g++
- meson

### Privilege escalation dependencies

To correctly handle priviledge escalation an SSH askpass package needs to be installed.  
Here are three possible packages available on the Ubuntu distro by default:  

```bash
- sudo apt install ssh-askpass # or
- sudo apt install ssh-askpass-fullscreen # or
- sudo apt install ssh-askpass-gnome
```

Another solution is it to have root access on your local machine without being prompted for gaining priviledge (aka passwordless root access), this is only relevant / recommended for a development setup.  

## Debian/Ubuntu Installation Instructions

After cloning the project and installing all dependencies listed below, simply call 

```bash
bash bash-developer/run-reinstall--appio-terminal--local.sh
```

at the project root.

### Dependencies

APPIO is built on dotnet 2.1. Install it by following the instructions by Microsoft: https://dotnet.microsoft.com/download/linux-package-manager/ubuntu19-04/sdk-2.1.700. 

Certain features of APPIO require additional dependencies:

- Python with Meson & GCC for building projects
- Python3 for information model management
- OpenSSL for certificate management

---

You can print the debian package information for APPIO by calling

```bash
dpkg --list | grep appio-terminal
```

and you can purge APPIO with

```bash
dpkg --purge appio-terminal
```

## Documentation

Documentation can be found at https://appioframework.readthedocs.io

## Contributing

Please use GitHub's issue and pull request system for submitting bugs, suggestions and changes to APPIO.

If you want to contribute to APPIO, you will need further development dependencies, namely the dotnet SDK v2.1, the meson build system, ninja build, gcc and a SSH askpass.

The installation instructions for the dotnet SDK can be found here: https://dotnet.microsoft.com/download/linux-package-manager/ubuntu19-04/sdk-2.1.700.

For SSH askpass there are three packages available:

- `ssh-askpass`
- `ssh-askpass-fullscreen`
- `ssh-askpass-gnome`

Just install one of them:  

```bash
apt-get update  
apt-get install <ssh-askpass-package-name>
```

## License

APPIO is released under the Mozilla Public License. You can find details in [LICENCE](LICENCE).

