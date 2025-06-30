using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;

namespace BoTech.StarClock.Helper;

public class UpdateManager
{
    public readonly string VersionNumber = "1.0.1";
    private string _currentVersionDebFileName = String.Empty;

    public bool CheckForUpdate()
    {
        /*
         * CurrentReleaseVersion.txt example Content.
         * 1.2.27
         * BoTech.StarClock_1.2.27_arm64.deb
         */
        string? fileContent = null;
        if ((fileContent =
                GetContentOfWebFile("https://debian.botech.dev/BoTech.StarClock/CurrentReleaseVersion.txt")) != null)
        {
            string[] lines = fileContent.Split('\n');
            if (lines.Length == 2)
            {
                if (!lines[0].Equals(VersionNumber))
                {
                    _currentVersionDebFileName = lines[1];
                    return true;
                }
            }
        }
        // Error or no update
        return false;
    }

    public void InstallUpdate()
    {
        string error;
        ExecuteCommand("sh /usr/lib/BoTech.StarClock/Update.sh", out error);
    }
    private string? GetContentOfWebFile(string url)
    {
        HttpClient client = new HttpClient();
        
        try
        {
            byte[] fileBytes = client.GetByteArrayAsync(url).Result;
            return Encoding.UTF8.GetString(fileBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }
    
 /*   
    private DateTime? _lastCheckedForUpdates = null;
    private bool _appNeedsUpdate = false;
    private bool _systemNeedsUpdate = false;
    /// <summary>
    /// Checks for available system updates by running the "sudo apt update" command.
    /// </summary>
    /// <returns>
    /// Count of upgradable packages.
    /// </returns>
    public int CheckForSystemUpdates()
    {
        string output = ExecuteCommand("sudo apt update", out string error);
        output = ExecuteCommand("sudo apt list --upgradable", out error);
        if (error == string.Empty && !string.IsNullOrEmpty(output))
        {
            _lastCheckedForUpdates = DateTime.Now;
            output = output.Replace("Listening... Done\n", "");
            if(output.Split('\n').Length > 0) _systemNeedsUpdate = true;
            return output.Split('\n').Length;
        }
        _systemNeedsUpdate = false;
        return -1;
    }

    public bool DoesThisAppNeedAnUpdate()
    {
        string output = ExecuteCommand("sudo apt update", out string error);
        output = ExecuteCommand("sudo apt list --upgradable", out error);
        if (error == string.Empty && !string.IsNullOrEmpty(output))
        {
            // Now check if output contains botech.starclock
            _lastCheckedForUpdates = DateTime.Now;
            _appNeedsUpdate = output.Contains("botech.starclock");
            return output.Contains("botech.starclock");
        }
        _appNeedsUpdate = false;
        return false;
    }
    /// <summary>
    /// Installs all available updates.
    /// </summary>
    /// <returns></returns>
    public string? InstallUpdates()
    {
        // First, check whether the app or the system needs any updates and that the last check was not more than an hour ago
        if (!(_systemNeedsUpdate || _appNeedsUpdate) && (DateTime.Now - _lastCheckedForUpdates).Value.TotalMinutes < 60)
        {
            CheckForSystemUpdates();
            DoesThisAppNeedAnUpdate();
        }
        
        string output = ExecuteCommand("sudo apt upgrade", out string error);
        // Restart after the update to apply it.
        output += ExecuteCommand("sudo restart now", out error);
        return output;
    }
*/
    private string? ExecuteCommand(string command, out string error)
    {
        // Create a new process
        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash", // Use bash to execute the command
                Arguments = $"-c \"{command}\"", // Pass the command as an argument
                RedirectStandardOutput = true, // Capture the output
                RedirectStandardError = true,  // Capture errors
                UseShellExecute = false,       
                CreateNoWindow = true,       
            }
        };
        
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        error = process.StandardError.ReadToEnd();

        // Wait for the process to exit
        process.WaitForExit();
        return output;
    }
}