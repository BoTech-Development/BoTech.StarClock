using System.Diagnostics;

namespace BoTech.StarClock.Helper;

public class TerminalRunner
{
    public static Process? StartTerminal(string command)
    {
        // Run inside a login-capable shell so PATH/aliases work; keep window open after.
        //string shellCommand = $"bash -lc '{EscapeSingleQuotes(command)}; echo; read -p \"Press Enter to close...\"'";
        string shellCommand = $"bash -lc '{EscapeSingleQuotes(command)};'";
    
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "/usr/bin/x-terminal-emulator", // Debian/Raspberry Pi wrapper
            UseShellExecute = false,
            CreateNoWindow = false
        };
        psi.ArgumentList.Add("-e");
        psi.ArgumentList.Add(shellCommand);

        return Process.Start(psi);
    }

    private static string EscapeSingleQuotes(string s) => s.Replace("'", "'\"'\"'");
}