FROM ubuntu:bionic

RUN apt-get update --yes                                                                                                                                   \
 && apt-get install --yes --no-install-recommends                                                                                                          \
                   gnupg                                                                                                                                   \
                   apt-transport-https                                                                                                                     \
                   software-properties-common                                                                                                              \
                   gcc                                                                                                                                     \
                   g++                                                                                                                                     \
                   meson                                                                                                                                   \
 && add-apt-repository universe -y                                                                                                                         \
 && apt-key adv --keyserver packages.microsoft.com --recv-keys EB3E94ADBE1229CF                                                                            \
 && apt-key adv --keyserver packages.microsoft.com --recv-keys 52E16F86FEE04B979B07E28DB02C46DF417A0893                                                    \
 && sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-bionic-prod bionic main" > /etc/apt/sources.list.d/dotnetdev.list' \
 && apt-get update --yes                                                                                                                                   \
 && apt-get install --yes --no-install-recommends                                                                                                          \
                   dotnet-sdk-2.1

