using System.Diagnostics;

namespace BoTech.StarClock.Elevator;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("libc")]
    private static extern uint geteuid();

    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: dotnet BoTech.StarClock.Elevator.dll <target.dll>");
            return;
        }

        var targetDll = args[0];

        if (geteuid() != 0)
        {
            Console.WriteLine("Not running as root. Attempting relaunch with sudo...");

            var psi = new ProcessStartInfo
            {
                FileName = "sudo",
                ArgumentList = { "dotnet", "/home/rpi/botech/bot.sc/BoTech.StarClock.Elevator.dll", targetDll },
                UseShellExecute = false
            };

            try
            {
                Process.Start(psi)?.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to relaunch with sudo: {ex.Message}");
            }

            return;
        }

        Console.WriteLine("Running with root privileges. Launching target DLL...");

        var runDll = new ProcessStartInfo
        {
            FileName = "dotnet",
            ArgumentList = { targetDll },
            UseShellExecute = false
        };

        try
        {
            Process.Start(runDll)?.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to launch target DLL: {ex.Message}");
        }
    }
}