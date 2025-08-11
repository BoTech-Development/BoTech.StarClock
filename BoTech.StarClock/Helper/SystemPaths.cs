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
}