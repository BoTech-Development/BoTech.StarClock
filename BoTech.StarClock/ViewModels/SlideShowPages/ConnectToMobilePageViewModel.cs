using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive;
using ReactiveUI;

namespace BoTech.StarClock.ViewModels.SlideShowPages;

public class ConnectToMobilePageViewModel : ViewModelBase
{
    public string IpAddress { get; set; }
    public ReactiveCommand<Unit, Unit> ReloadCommand { get; }
    public ReactiveCommand<Unit, Unit> CloseCommand { get; }

    public ConnectToMobilePageViewModel()
    {
        IpAddress = GetLocalIPAddress();
        ReloadCommand = ReactiveCommand.Create(() =>
        {
            this.IpAddress = GetLocalIPAddress();
        });
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
        List<IPAddress> ipAddresses = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
        IPAddress? ipAddress = null;
        foreach (IPAddress address in ipAddresses)
        {
            if (address.ToString() != "127.0.1.1")
            {
                ipAddress = address;
                break;
            }
        }
        if (ipAddress == null)
        {
            throw new Exception("No IPv4 network adapters with an IP address in the system.");
        }
        return ipAddress.ToString();
    }
}