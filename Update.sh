#!/bin/bash
echo "Update Script v1.0 (c) 2025 www.botech.dev All Rights Reserved!"
echo "Update for System and BoTech.StarClock will be installed."
echo "Please leave the computer switched on and connected to the Internet! "
echo "Otherwise your System could be damaged!"
wait 15 # Wait a moment until the application has shut down.
# First let update and upgrade Raspberry os lite
sudo apt update
sudo apt upgrade -y
sudo apt autoremove
# Then download the .deb file. The file Name will be passed by the application.
wget https://debian.botech.dev/BoTech.StarClock/"$0"
# unistalling the old version
sudo apt remove BoTech.StarClock
# installing the new version
sudo apt install ./$
#reboot and the app will start automatically through the service
sudo reboot now
