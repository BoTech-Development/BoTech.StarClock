using System;

namespace BoTech.StarClock.Helper;

public class SystemPaths
{
    public static string GetBaseInstallationPath()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData,
                Environment.SpecialFolderOption.Create);
        }
        else if(Environment.OSVersion.Platform == PlatformID.Unix)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop,
                Environment.SpecialFolderOption.Create);
        }

        return string.Empty;
    }
    /// <summary>
    /// Returns the Path where all data of the app can be stored.
    /// </summary>
    /// <returns></returns>
    public static string GetBaseProgramDataPath(string extensionToPath = "")
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            return ReturnPathAndCreate(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData,
                Environment.SpecialFolderOption.Create) + "/botech/bot.sc/AppData/" + extensionToPath);
        }
        else if(Environment.OSVersion.Platform == PlatformID.Unix)
        {
            return ReturnPathAndCreate(Environment.GetFolderPath(Environment.SpecialFolder.Desktop,
                Environment.SpecialFolderOption.Create) + "/botech/bot.sc/AppData/" + extensionToPath);
        }
        return string.Empty;
    }

    private static string ReturnPathAndCreate(string path)
    {
        if(!Directory.Exists(path)) Directory.CreateDirectory(path);
        return path;
    }
}