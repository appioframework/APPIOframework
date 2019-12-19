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

### Miscellaneous

Install instructions for dotnet SDK can be found here: https://dotnet.microsoft.com/download/linux-package-manager/ubuntu19-04/sdk-2.1.700.  
To easily install ansible and molecule on your development system follow the [travis CI install steps ('before_install' and 'install' sections)](.travis.yml).  

## Scripts

To run any of the following scripts locally open a terminal in the APPIOframework repository root and run listed commands.  

### Continuous Integration Scripts

The build and unit test step can be started with:  
```bash
/bin/bash bash/ci/run-pipeline.bash --build-and-test
```

The publish step can be started with:  
```bash
/bin/bash bash/ci/run-pipeline.bash --publish
```

The installer build step can be started with:  
```bash
/bin/bash bash/ci/run-pipeline.bash --build-installer
```

The system test step can be started with:  
```bash
/bin/bash bash/ci/run-pipeline.bash --system-test
```

The full pipeline can be started with:  
```bash
/bin/bash bash/ci/run-pipeline.bash # or
/bin/bash bash/ci/run-pipeline.bash --full-pipeline # or
/bin/bash bash/ci/run-pipeline.bash --build-and-test --publish --build-installer --system-test
```

### Development Scripts

The publish step can be started with:  
```bash
bash bash/development/run-reinstall-appio-terminal.bash --publish
```

The installer build step can be started with:  
```bash
/bin/bash bash/development/run-reinstall-appio-terminal.bash --build-installer
```

The uninstall appio step can be started with:  
```bash
bash bash/development/run-reinstall-appio-terminal.bash --remove
```

The install appio step can be started with:  
```bash
bash bash/development/run-reinstall-appio-terminal.bash --install
```

The check appio step can be started with:  
```bash
bash bash/development/run-reinstall-appio-terminal.bash --help
```

The full pipeline can be started with:  
```bash
/bin/bash bash/development/run-reinstall-appio-terminal.bash # or
/bin/bash bash/development/run-reinstall-appio-terminal.bash --full-pipeline # or
/bin/bash bash/development/run-reinstall-appio-terminal.bash --publish --build-installer --remove --install --help
```

## System Inspection

You can print the debian package information for APPIO by calling

```bash
dpkg --list | grep appio-terminal
```

and you can purge APPIO manually with

```bash
dpkg --purge appio-terminal
```

## Documentation

Documentation can be found at https://appioframework.readthedocs.io

## Contributing

Please use GitHub's issue and pull request system for submitting bugs, suggestions and changes to APPIO.  

If you want to contribute to APPIO, you will need further development [dependencies](#dependencies).  

## License

APPIO is released under the Mozilla Public License v2. You can find details in [LICENCE](LICENCE).  
