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
    /// <summary>
    /// Home Path for all BoTech products and temp/AppData files
    /// old => CommonDocuments
    /// </summary>
    private static readonly string HomePath = SystemPaths.GetBaseInstallationPath() + "/botech/";
    /// <summary>
    /// The Standard installation folder
    /// </summary>
    private static readonly string ProductPath = SystemPaths.GetBaseInstallationPath() + "/botech/bot.sc/";
    /// <summary>
    /// Temp folder for Updating the products
    /// </summary>
    private static readonly string UpdatePath = SystemPaths.GetBaseInstallationPath() + "/botech/temp/update/";


    private GitHubClient _client;
    private string _currentVersionDebFileName = String.Empty;
    
    /// <summary>
    /// The hardcoded Version Number of this Product
    /// </summary>
    public static readonly VersionInfo ThisVersion = new VersionInfo(new Version(1,0,4))
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
        set
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
        set
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
    public UpdateManager()
    {
        _client = new GitHubClient(new ProductHeaderValue("BoTech.StarClock_" + ThisVersion.GetVersionStringWithoutDateTime()));
        Console.WriteLine("------Folder:------");
        Console.WriteLine(HomePath);
        Console.WriteLine(ProductPath);
        Console.WriteLine(UpdatePath);
        Console.WriteLine("------------");
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
            List<string> oldInstallations = Directory.EnumerateDirectories("/home/rpi/Desktop/botech/bot.sc/").ToList();
            foreach (string installation in oldInstallations)
            {
                string versionString = installation.Replace("/home/rpi/Desktop/botech/bot.sc/", "").Replace("/", "");
                if (!versionString.Equals(ThisVersion.GetVersionStringWithoutDateTime()))
                {
                    Directory.Delete(installation, true);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error by deleting old Versions!!!");
            Console.WriteLine(e);
            Console.WriteLine("-----------------------------------");
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
        if (stableReleases.Count > 0 && stableReleases[0].VersionInfo.IsLowerThanThis(ThisVersion))
        {
            NextUpdate = stableReleases[0];
            IsNextUpdateUnstable = false;
            return true;
        }
        if (unstableReleases.Count > 0 && unstableReleases[0].VersionInfo.IsLowerThanThis(ThisVersion))
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
        ReleaseAsset? updateAsset = null;
        FileDownloader downloader = new FileDownloader();
        downloader.FileProgressChanged += DownloaderOnFileProgressChanged;
        if (DownloadUpdate(NextUpdate,  out updateAsset) && updateAsset != null)
        {
            ExtractUpdate(updateAsset);
            CleanUpDirectory(UpdatePath);
            CurrentProgress = 100;
            IsProgressBarIndeterminate = false;
            return true;
        }
        return false;
        Environment.Exit(0); // Shut app Down
    }



    /// <summary>
    /// Extracts all files and moves the AutoStar.sh script.
    /// </summary>
    /// <returns></returns>
    private void ExtractUpdate(ReleaseAsset updateAsset)
    {
        CurrentStatus = "Preparing to extract/install update...";
        IsProgressBarIndeterminate = true;
        Directory.CreateDirectory(ProductPath + NextUpdate.VersionInfo.GetVersionStringWithoutDateTime() + "/");
        IsProgressBarIndeterminate = false;
        new ZipExtractor(this).ExtractZipWithProgress(UpdatePath + updateAsset.Name, ProductPath + NextUpdate.VersionInfo.GetVersionStringWithoutDateTime() + "/", NextUpdate.VersionInfo.ToString());
        
        CurrentStatus = "Installing AutoStart.py ...";
        IsProgressBarIndeterminate = true;
        File.Delete(ProductPath + "AutoStart.py");
        File.Move(UpdatePath + "AutoStart.py", ProductPath + "AutoStart.py");
    }
  
    /// <summary>
    /// Downloads all necessary Files for the Update
    /// </summary>
    /// <param name="release"></param>
    /// <param name="updateAsset">This out parameter will be populated with information about the .zip file, when this function returns true.</param>
    /// <returns>false when an error occured.</returns>
    private bool DownloadUpdate(VersionedGitRelease release, out ReleaseAsset? updateAsset)
    {
        CurrentStatus = "Prepare download...";
        IsProgressBarIndeterminate = true;
        Console.WriteLine("Preparing Download...");
        // There must be a zip File that contains the contents of the publish folder.
        updateAsset = release.Release.Assets.Where(r => r.Name.Equals(release.VersionInfo.GetVersionStringWithoutDateTime() + "_arm64.zip")).FirstOrDefault();
        // There must be a .sh File which contains the dotnet command which starts the app => Example: dotnet /home/rpi/botech/bot.sc/v2.3.16.Alpha.LTS-(01.01.2030_12:12:12)/BoTech.StarClock.Desktop.dll
        ReleaseAsset? autostartAsset = release.Release.Assets.Where(r => r.Name.Equals("AutoStart.py")).FirstOrDefault();
        
        Console.WriteLine(release.VersionInfo.GetVersionStringWithoutDateTime());
        Console.WriteLine(updateAsset?.Name);
        Console.WriteLine(autostartAsset?.Name);

        FileDownloader downloader = new FileDownloader();
        downloader.FileProgressChanged += DownloaderOnFileProgressChanged;
        
        if (updateAsset != null && autostartAsset != null)
        {
            Console.WriteLine("Attempting to cleanup update directory...");
            // Cleanup the update Directory => normally the app does it after the update, but it is saver to do it twice.
            CleanUpDirectory(UpdatePath);
            // Now let's download the update file to a temp directory
            Console.WriteLine("Downloading {0} to {1}", updateAsset.Name, UpdatePath + "/" + updateAsset.Name);
            if (downloader.DownloadFileWithProgress(updateAsset.BrowserDownloadUrl, UpdatePath + "/" + updateAsset.Name)) //DownloadFileWithProgress(updateAsset.BrowserDownloadUrl.Replace(updateAsset.Name, ""), updateAsset.Name, UpdatePath, "Binaries").Result)
            {
                Console.WriteLine("Downloading {0} to {1}", autostartAsset.Name, UpdatePath + "/" + autostartAsset.Name);
                // Download the AutoStart.sh File
                if ( downloader.DownloadFileWithProgress(autostartAsset.BrowserDownloadUrl, UpdatePath + "/" + autostartAsset.Name)) //DownloadFileWithProgress(autostartAsset.BrowserDownloadUrl.Replace(autostartAsset.Name, ""), autostartAsset.Name, UpdatePath, "AutoStart script").Result)
                {
                    return true;
                }
            }
           
        }
        return false;
    }
    /// <summary>
    /// Event will be called when an update file will be downloaded.
    /// </summary>
    /// <param name="e"></param>
    private void DownloaderOnFileProgressChanged(DownloadProgressChangedEventArgs e)
    {
        IsProgressBarIndeterminate = false;
        CurrentProgress = e.Percentage;
        CurrentStatus = "Downloading " + e.ProgressText + " (" + e.BytesReceived + "/" + e.TotalBytesToReceive + ") " + e.Percentage + "%";
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
            Console.WriteLine($"Directory {directory} does not exist! Creating Directory {directory}");
            Directory.CreateDirectory(directory);
        }
        else
        {
            foreach (string file in Directory.EnumerateFiles(directory))
            {
                Console.WriteLine($"File {file} will be deleted!");
                new FileInfo(file).Delete();
            }
            foreach (string subDir in Directory.EnumerateDirectories(directory))
            {
                Console.WriteLine($"Sub-Directory {subDir} will be deleted!");
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
}