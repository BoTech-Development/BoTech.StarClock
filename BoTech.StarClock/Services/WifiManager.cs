using System;
using System.Management;

namespace BoTech.StarClock.Services;

public class WifiManager
{
    public bool TryToConnectToWifi()
    {

        string ssid = "YourNetworkSSID"; // Replace with your Wi-Fi SSID
        string password = "YourNetworkPassword"; // Replace with your Wi-Fi password

        try
        {
            // Create a Wi-Fi profile XML
            string profileXml = $@"
            <WLANProfile xmlns='http://www.microsoft.com/networking/WLAN/profile/v1'>
                <name>{ssid}</name>
                <SSIDConfig>
                    <SSID>
                        <name>{ssid}</name>
                    </SSID>
                </SSIDConfig>
                <connectionType>ESS</connectionType>
                <connectionMode>auto</connectionMode>
                <MSM>
                    <security>
                        <authEncryption>
                            <authentication>WPA2PSK</authentication>
                            <encryption>AES</encryption>
                            <useOneX>false</useOneX>
                        </authEncryption>
                        <sharedKey>
                            <keyType>passPhrase</keyType>
                            <protected>false</protected>
                            <keyMaterial>{password}</keyMaterial>
                        </sharedKey>
                    </security>
                </MSM>
            </WLANProfile>";

            // Use WMI to add the profile and connect
            ManagementClass wlanManager = new ManagementClass("root\\StandardCimv2", "MSFT_WiFiNetwork", null);
            foreach (ManagementObject wlan in wlanManager.GetInstances())
            {
                wlan.InvokeMethod("AddProfile", new object[] { profileXml, true });
                wlan.InvokeMethod("Connect", new object[] { "ESS", ssid });
                Console.WriteLine($"Connected to {ssid}");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }

    }
}
