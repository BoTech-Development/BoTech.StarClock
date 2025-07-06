using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BoTech.StarClock.Models.Update;
using Octokit;
using FileMode = System.IO.FileMode;

namespace BoTech.StarClock.Helper;

public delegate void CurrentStatusChanged(UpdateManager updaterInstance, string currentStatus);
public class UpdateManager
{
    public UpdateManager()
    {
        _client = new GitHubClient(new ProductHeaderValue("BoTech.StarClock_" + ThisVersion));
    }

    private GitHubClient _client;
    private string _currentVersionDebFileName = String.Empty;
    
    /// <summary>
    /// The hardcoded Version Number of this Product
    /// </summary>
    public readonly VersionInfo ThisVersion = new VersionInfo(new Version(1,0,1))
    {
        IsAlpha = true
    };
    public VersionedGitRelease NextUpdate { get; private set; }
    public bool IsNextUpdateUnstable { get; private set; } = false;
    /// <summary>
    /// <see cref="CurrentStatus"/> needs this field for getter and setter.
    /// </summary>
    private string _currentStatus = "Please initialize a new Instance or perform an action.";
    /// <summary>
    /// Text based status information like: "Downloading Update (v2.3.16.Alpha.LTS-(01.01.2030_12:12:12)) 36MB / 110MB"
    /// </summary>
    public string CurrentStatus
    {
        get => _currentStatus;
        private set
        {
            _currentStatus = value;
            OnCurrentStatusChanged(this, _currentStatus);
        } 
    } 
    /// <summary>
    /// This event will be invoked when the CurrentStatus string was changed.
    /// </summary>
    public event CurrentStatusChanged OnCurrentStatusChanged;
    /// <summary>
    /// <see cref="CurrentProgress"/> needs this field for getter and setter.
    /// </summary>
    private int _currentProgress = 0;
    /// <summary>
    /// Saves the current progress (percentage) of the current progress.
    /// </summary>
    public int CurrentProgress
    {
        get => _currentProgress;
        private set
        {
            _currentProgress =  value;
            OnCurrentStatusChanged(this, _currentStatus);
        }
    } 
    private bool _isProgressBarIndeterminate = false;

    public bool IsProgressBarIndeterminate
    {
        get => _isProgressBarIndeterminate;
        set
        {
            _isProgressBarIndeterminate = value;
            OnCurrentStatusChanged(this, _currentStatus);
        }
    }
    /// <summary>
    /// Deletes all old installed Versions of this Software, because during an update it isn't possible to delete all file of the current Version
    /// => That would mean that the software deletes its .dll or .exe file while it runs 
    /// </summary>
    public void DeleteOldInstalledVersions()
    {
        CurrentStatus = "Deleting old installation do not shutdown!!!";
        try
        {
            List<string> oldInstallations = Directory.EnumerateDirectories("/home/rpi/botech/bot.sc/").ToList();
            foreach (string installation in oldInstallations)
            {
                string versionString = installation.Replace("/home/rpi/botech/bot.sc/", "").Replace("/", "");
                VersionInfo? oldVersion = null;
                if ((oldVersion = VersionInfo.Parse(versionString)) != null)
                {
                    if (!oldVersion.Equals(ThisVersion))
                    {
                        Directory.Delete(installation, true);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        CurrentStatus = "Ready.";
    }
    public bool CheckForUpdate()
    {
        try
        {
            return TryToCheckForUpdates();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new("Can not get either the stable or the unstable Releases from the Update server!");
        }
    }
    private bool TryToCheckForUpdates()
    {
        CurrentStatus = "Checking for Updates...";
        IsProgressBarIndeterminate = true;
        List<VersionedGitRelease> stableReleases = new List<VersionedGitRelease>();
        GetStableReleases(stableReleases);
        List<VersionedGitRelease> unstableReleases = new List<VersionedGitRelease>();
        GetUnstableReleases(unstableReleases);
        // ascending sorting by date of publication
        stableReleases.Sort((x,y) => x.Release.CreatedAt.CompareTo(y.Release.CreatedAt));
        unstableReleases.Sort((x,y) => x.Release.CreatedAt.CompareTo(y.Release.CreatedAt));
        
        // Check if there is a stable Update available
        if (stableReleases[0].VersionInfo.IsLowerThanThis(ThisVersion))
        {
            NextUpdate = stableReleases[0];
            IsNextUpdateUnstable = false;
            return true;
        }
        if (unstableReleases[0].VersionInfo.IsLowerThanThis(ThisVersion))
        {
            NextUpdate = unstableReleases[0];
            IsNextUpdateUnstable = true;
            return true;
        }
        return false;
    }
    /// <summary>
    /// List all stable releases from the git repository
    /// </summary>
    /// <param name="result">The List will be populated, please insert an empty list.</param>
    private void GetStableReleases(List<VersionedGitRelease> result)
    {
        List<Release> releases = _client.Repository.Release.GetAll("BoTech-Development", "BoTech.StarClock").Result.ToList();
        result.AddRange(ConnectAndCreateVersionInfosForReleases(releases));
    }
    /// <summary>
    /// List all unstable releases from the git repository
    /// </summary>
    /// <param name="result">The List will be populated, please insert an empty list.</param>
    private void GetUnstableReleases(List<VersionedGitRelease> result)
    {
        List<Release> releases = _client.Repository.Release.GetAll("BoTech-Development", "BoTech.StarClock-Unstable").Result.ToList();
        result.AddRange(ConnectAndCreateVersionInfosForReleases(releases));
    }
    /// <summary>
    /// Connects all fetched releases to their Version Model, which will be created from the UpdateInfo.txt file (which is backwards compatible) in the release assets.
    /// </summary>
    /// <param name="releases">fetched releases from the GitHub api</param>
    /// <returns>the new list or null when an error occured</returns>
    private List<VersionedGitRelease>? ConnectAndCreateVersionInfosForReleases(List<Release> releases)
    {
        List<VersionedGitRelease> connectedReleases = new List<VersionedGitRelease>();
        foreach (Release release in releases)
        {
            string? fileContent = GetUpdateInfoForRelease(release);
            if (fileContent != null)
            {
                VersionInfo? versionInfo = null;
                if((versionInfo = ParseUpdateInfoFile(fileContent)) != null)
                    connectedReleases.Add(new VersionedGitRelease(versionInfo, release));
            }
        }
        return connectedReleases; 
    }
    /// <summary>
    /// Takes the content of an UpdateInfo file and parse it by using the VersionInfo class.
    /// This Method first finds the VersionString format which can be parsed by the current Version of VersionInfo.
    /// </summary>
    /// <param name="fileContent">The Content of the UpdateInfo file.</param>
    /// <returns>The parsed version Info, but null when the VersionString is invalid or the file content isn't correct.</returns>
    /// <exception cref="ArgumentNullException">Only when the programmer did not read the docs!</exception>
    private VersionInfo? ParseUpdateInfoFile(string fileContent)
    {
        // Will be always false because the parent method checks if the file Content is null.
        if(fileContent == null) throw new ArgumentNullException(nameof(fileContent));
        string[] lines = fileContent.Split('\n');
        //Remove all \r
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Replace("\r", "");
        }
        // get the string with the correct format version
        for (int i = 0; i < lines.Length; i++)
            if (lines[i].Equals(VersionInfo.FormatVersion + "=>"))
            {
                if (i + 1 < lines.Length)
                {
                    return VersionInfo.Parse(lines[i + 1]);
                }
            }
        return null;
    }
    /// <summary>
    /// Tries to download and read the UpdateInfo File for a Release.
    /// </summary>
    /// <param name="release">The release</param>
    /// <returns>The Content of the file or null when an error occured.</returns>
    private string? GetUpdateInfoForRelease(Release release)
    {
        // This File contains the Version Number of this Release which can be parsed by the VersionInfo class.
        ReleaseAsset? updateInfoFile = release.Assets.Where(a => a.Name.Equals("UpdateInfo.txt")).FirstOrDefault();
        if (updateInfoFile != null)
        {
            return GetContentOfWebFile(updateInfoFile.BrowserDownloadUrl);
        }
        return null;
    }
    /// <summary>
    /// Caution you must run <see cref="CheckForUpdate"/> before this Method.
    /// This Method will execute the Update script and shut this app down before.
    /// </summary>
    /// <returns>False when an error occurred</returns>
    public bool InstallUpdate()
    {
        if (DownloadUpdate(NextUpdate))
        {
            if (ExtractUpdate())
            {
                return true;
            }
        }
        return false;
        Environment.Exit(0); // Shut app Down
    }
    /// <summary>
    /// Extracts all files and moves the AutoStar.sh script.
    /// </summary>
    /// <returns></returns>
    private bool ExtractUpdate()
    {
        CurrentStatus = "Preparing to extract/install update...";
        IsProgressBarIndeterminate = true;
        Directory.CreateDirectory("/home/rpi/botech/bot.sc/" + NextUpdate.VersionInfo.ToString() + "/");
        ReleaseAsset? updateAsset = NextUpdate.Release.Assets.Where(r => r.Name.Equals(NextUpdate.VersionInfo.ToString() + "_arm64.zip")).FirstOrDefault();
        if (updateAsset != null)
        {
            ExtractZipWithProgress("/home/rpi/botech/temp/update/" + updateAsset.Name,
                "/home/rpi/botech/bot.sc/" + NextUpdate.VersionInfo + "/", NextUpdate.VersionInfo.ToString());
            File.Delete("/home/rpi/botech/bot.sc/AutoStart.sh");
            File.Move("/home/rpi/botech/temp/update/AutoStart.sh", "/home/rpi/botech/bot.sc/AutoStart.sh");
            return true;
        }

        return false;
    }
    /// <summary>
    /// Extracts any .zip file to a specific folder.
    /// </summary>
    /// <param name="zipPath">path to the zip file</param>
    /// <param name="extractPath">the destination folder</param>
    /// <param name="progressText">The string which should be shown.</param>
    private void ExtractZipWithProgress(string zipPath, string extractPath, string progressText)
    {
        using (ZipArchive archive = ZipFile.OpenRead(zipPath))
        {
            int totalEntries = archive.Entries.Count;
            int processedEntries = 0;

            foreach (var entry in archive.Entries)
            {
                string destinationPath = Path.Combine(extractPath, entry.FullName);

                if (string.IsNullOrEmpty(entry.Name)) // Handle directories
                {
                    Directory.CreateDirectory(destinationPath);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                    entry.ExtractToFile(destinationPath, overwrite: true);
                }

                processedEntries++;
                CurrentProgress = (int)((double)processedEntries / totalEntries * 100);
                CurrentStatus = "Extracting " + progressText + " (" + processedEntries + "/" + totalEntries + ")";
            }
        }

    }
    /// <summary>
    /// Downloads all necessary Files for the Update
    /// </summary>
    /// <param name="release"></param>
    /// <returns>false when an error occured.</returns>
    private bool DownloadUpdate(VersionedGitRelease release)
    {
        CurrentStatus = "Prepare download...";
        IsProgressBarIndeterminate = true;
        // There must be a zip File which contains the contents of the publish folder.
        ReleaseAsset? updateAsset = release.Release.Assets.Where(r => r.Name.Equals(release.VersionInfo.ToString() + "_arm64.zip")).FirstOrDefault();
        // There must be a .sh File which contains the dotnet command which starts the app => Example: dotnet /home/rpi/botech/bot.sc/v2.3.16.Alpha.LTS-(01.01.2030_12:12:12)/BoTech.StarClock.Desktop.dll
        ReleaseAsset? autostartAsset = release.Release.Assets.Where(r => r.Name.Equals("AutoStart.sh")).FirstOrDefault();
        if (updateAsset != null && autostartAsset != null)
        {
            // Cleanup the update Directory => normally the app do it after the update but it is saver to do it twice.
            CleanUpDirectory("/home/rpi/botech/temp/update");
            // Now let's download the update file to a temp directory
            if (DownloadFileWithProgress(updateAsset.BrowserDownloadUrl.Replace(updateAsset.Name, ""), updateAsset.Name,
                    "/home/rpi/botech/temp/update/", "Binaries").Result)
            {
                // Download the AutoStart.sh File
                if (DownloadFileWithProgress(autostartAsset.BrowserDownloadUrl.Replace(autostartAsset.Name, ""),
                        autostartAsset.Name, "/home/rpi/botech/temp/update/", "AutoStart script").Result)
                {
                    return true;
                }
            }
           
        }
        return false;
    }
    /// <summary>
    /// Deletes the content of a directory and all Subdirectories.
    /// </summary>
    /// <param name="directory">The directory which should be empty.</param>
    private void CleanUpDirectory(string directory)
    {
        CurrentStatus = "Cleaning the update directory...";
        IsProgressBarIndeterminate = true;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        else
        {
            foreach (string file in Directory.EnumerateFiles(directory))
            {
                new FileInfo(file).Delete();
            }
            foreach (string subDir in Directory.EnumerateDirectories(directory))
            {
                CleanUpDirectory(subDir);
            }
        }
    }
    /// <summary>
    /// This Method downloads a File from the Web and reads the content and converts it into a string.
    /// </summary>
    /// <param name="url">The Url of the File which should be downloaded.</param>
    /// <returns>The Content of the downloaded File.</returns>
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
    
    /// <summary>
    /// Downloads a file form a web-path to a specific location on the device
    /// </summary>
    /// <param name="baseUrl">The download Url without the file extension.</param>
    /// <param name="fileName">The Filename which should be downloaded.</param>
    /// <param name="destination">The Destination folder</param>
    /// <param name="progressText">The Info, which will be placed between the word Download and the download percentage</param>
    /// <returns>true when the file was successfully downloaded.</returns>
    private async Task<bool> DownloadFileWithProgress(string baseUrl, string fileName, string destination, string progressText)
    {
        try
        {
            await DownloadFileWithProgressUnsafe(baseUrl, fileName, destination, progressText);   
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    /// <summary>
    /// <c>Unsafe!!! can throw exceptions</c>
    /// Downloads a file form a web-path to a specific location on the device
    /// </summary>
    /// <param name="baseUrl">The download Url without the file extension.</param>
    /// <param name="fileName">The Filename which should be downloaded.</param>
    /// <param name="destination">The Destination folder</param>
    /// <param name="progressText">The Info, which will be placed between the word Download and the download percentage</param>
    private async Task DownloadFileWithProgressUnsafe(string baseUrl, string fileName, string destination,
        string progressText)
    {
        HttpClient client = new HttpClient();
        HttpResponseMessage response =
            await client.GetAsync(baseUrl + "/" + fileName, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        long? totalBytes = response.Content.Headers.ContentLength;
        Stream contentStream = await response.Content.ReadAsStreamAsync(),
            fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
        var buffer = new byte[8192];
        long totalBytesRead = 0;
        int bytesRead;
        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await fileStream.WriteAsync(buffer, 0, bytesRead);
            totalBytesRead += bytesRead;

            if (totalBytes.HasValue)
            {
                CurrentProgress = (int)((double)totalBytesRead / totalBytes.Value * 100);
                CurrentStatus = "Downloading " + progressText + "(" + totalBytesRead + "/" + totalBytes.Value + ")";
            }
        }
    }

    /// <summary>
    /// Helper method for executing bash commands
    /// </summary>
    /// <param name="command"></param>
    /// <param name="error"></param>
    /// <returns></returns>
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