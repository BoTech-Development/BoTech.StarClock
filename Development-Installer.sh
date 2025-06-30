#!/bin/bash

echo "BoTech.StarClock Installer for Development Purposes Only v1.0 (24.06.2025)"
echo "Installing DotNet"

curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel STS
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc

echo "Installing Missing Libraries"

sudo apt update
sudo apt upgrade
sudo apt-get install libgbm1 libgl1-mesa-dri libegl1-mesa libinput10

echo "Downloading BoTech.StarClock"

wget https://cdn.botech.dev/BoTech-StarClock.zip
mkdir BoTech.StarClock

echo "Unzipping BoTech.StarClock"

unzip BoTech-StarClock.zip -d ~/BoTech.StarClock

echo "Starting BoTech.StarClock"

cd BoTech.StarClock
dotnet BoTech.StarClock.Desktop.dll --drm