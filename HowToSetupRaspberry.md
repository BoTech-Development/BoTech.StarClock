# How to setup the Raspberry Zero 2w
## Display
1. Add the following lines to the end of the file /boot/firmware/config.txt

```
hdmi_group=2
hdmi_mode=87
hdmi_cvt 800 480 60 6 0 0 0
hdmi_drive=1
dtoverlay=waveshare-ads7846,penirq=25,xmin=200,xmax=3900,ymin=200,ymax=3900,speed=50000
```

2. Download the [waveshare-ads7846.dtbo](https://github.com/BoTech-Development/BoTech.StarClock/tree/master/ReadmeAssets/waveshare-ads7846.dtbo) file . Copy these file to the overlays directory (/boot/overlays/).
3. Add the following line to this file with an space char to the other text. /boot/firmware/cmdline.txt

```video=HDMI-A-1:1920x1080M@60D```
4. Reboot now
### Sources:
   + https://www.waveshare.com/wiki/5inch_HDMI_LCD
   + https://forums.raspberrypi.com/viewtopic.php?t=357687
## Application 
### Install dotnet runtime
````sh
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel STS
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc
````

### Install the App 
Create Directories and download binaries.:
````SH
mkdir botech && cd botech
mkdir bot.sc && cd bot.sc
mkdir !!!Insert Version String from the UpdateInfo.txt file here!!! && cd ??? # Example: mkdir 1.0.2.Alpha-[06.07.2025_13:08:18] && cd 1.0.2.Alpha-[06.07.2025_13:08:18]
wget https://github.com/BoTech-Development/BoTech.StarClock-Unstable/releases/download/v1.0.2/1.0.2.Alpha_arm64.zip # Change to the current release file!
unzip 1.0.2.Alpha_arm64.zip
mv publish/* . # Only necessare when there is an subfolder after unzipping th zip file.
cd ~/botech/bot.sc/ # Installing the AutoStart.sh Script. please use the newsest version too.
wget https://github.com/BoTech-Development/BoTech.StarClock-Unstable/releases/download/v1.0.2/AutoStart.sh
````