# oppo - oppo-terminal

The terminal project used to create / build and deploy oppo applications.

[![build status](https://git.talsen.team/TTAF/oppo-terminal/badges/master/build.svg)](https://git.talsen.team/TTAF/oppo-terminal/badges/master/build.svg)
[![coverage report](https://git.talsen.team/TTAF/oppo-terminal/badges/master/coverage.svg)](https://git.talsen.team/TTAF/oppo-terminal/badges/master/coverage.svg)

## debian installer - local developer build

1. Navigate with the terminal to the project root.
2. Call  
   /bin/bash bash-developer/run-build-installer-local.sh
3. The debian installer can be found in  
   installer/oppo-terminal.deb

## debian installer - local developer re-install

1. Ensure that a SSH askpass is installed.  
   There are three packages available:
   - ssh-askpass
   - ssh-askpass-fullscreen
   - ssh-askpass-gnome
   Just install one of them:  
   sudo apt-get update
   sudo apt-get install &lt;ssh-askpass-package-name&gt;
2. Navigate with the terminal to the project root.
3. Call  
   /bin/bash bash-developer/run-reinstall-oppo-local.sh
4. Call  
   dpkg --list | grep "oppo-terminal"  
   to print the debian package information for oppo terminal

## debian installer - purge oppo-terminal package manually

1. Call 
   dpkg --list | grep "oppo-*"  
   to print the debian package information for oppo terminal (and possibly other oppo packages)
2. Call  
   sudo dpkg --purge &lt;oppo-terminal-package-name&gt;
