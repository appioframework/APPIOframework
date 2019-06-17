# APPIO framework
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/appioframework/APPIOframework.svg)

## Adaptable Plug & Produce Industrial Orchestration Framework

<a href="http://apppioframework.org" target="_blank">
<img style="float: right;" src="logo.png">
</a>

**APPIO is a command-line utility for managing an OPC UA infrastructure. It is built on [open62541](https://github.com/open62541/open62541/), an open-source implementation of OPC UA, and it is published under the Mozilla Public License.**

## Debian/Ubuntu Installation Instructions

After cloning the project and installing all dependencies listed below, simply call 

```bash
bash bash-developer/run-reinstall--appio-terminal--local.sh
```

at the project root.

### Dependencies

APPIO is built on dotnet 2.1. Install it by following the instructions by Microsoft: https://dotnet.microsoft.com/download/linux-package-manager/ubuntu19-04/runtime-2.1.11 

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

If you want to contribute to APPIO, you will need further development dependencies, namely the dotnet SDK and SSH askpass.

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
