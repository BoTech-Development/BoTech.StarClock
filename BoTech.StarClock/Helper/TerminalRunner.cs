using System.Diagnostics;

namespace BoTech.StarClock.Helper;

public class TerminalRunner
{
    public static void StartTerminal(string command)
    {
     /*   // Run inside a login-capable shell so PATH/aliases work; keep window open after.
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

        return Process.Start(psi);*/
        string terminalCommand = "-- bash -c \"";
        terminalCommand += command;
        terminalCommand += " && exit;exec bash\"";
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "/bin/gnome-terminal",
            Arguments = terminalCommand,
            UseShellExecute = false,
            CreateNoWindow = false,
        };
        Process? mainProcess = Process.Start(startInfo);
        if(mainProcess != null)
            mainProcess.WaitForExit();
        Process? terminalProcess = null;
        Process[] processes = Process.GetProcessesByName("gnome-terminal-server");
        if (processes.Length > 0)
        {
            for(int i = 0; i < processes.Length; i++)
                if (processes[i].ProcessName == "gnome-terminal-server")
                {
                    terminalProcess = processes[i];
                    break;
                }
        }
        if(terminalProcess != null)
            terminalProcess.WaitForExit();
    }

    private static string EscapeSingleQuotes(string s) => s.Replace("'", "'\"'\"'");
}