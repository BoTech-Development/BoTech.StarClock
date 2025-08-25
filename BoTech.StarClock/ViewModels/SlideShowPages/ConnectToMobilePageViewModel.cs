using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace BoTech.StarClock.ViewModels.SlideShowPages;

public class ConnectToMobilePageViewModel : ViewModelBase
{
    public string IpAddress { get; set; }

    public ConnectToMobilePageViewModel()
    {
        IpAddress = GetLocalIPAddress();
    }
    /// <summary>
    /// Retrieves the local IP address of the machine.
    /// </summary>
    /// <returns>The local IP address as a string.</returns>
    static string GetLocalIPAddress()
    {
        // Get all network interfaces and their IP addresses
        var host = Dns.GetHostEntry(Dns.GetHostName());

        // Filter out loopback and IPv6 addresses
        var ipAddress = host.AddressList
            .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

        if (ipAddress == null)
        {
            throw new Exception("No IPv4 network adapters with an IP address in the system.");
        }

        return ipAddress.ToString();
    }
}