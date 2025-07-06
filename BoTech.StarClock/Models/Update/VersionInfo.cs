using System;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ReactiveUI;


namespace BoTech.StarClock.Models.Update;

public class VersionInfo
{
    // ID is not used in this Program
    //public int Id { get; set; }
    public static readonly string FormatVersion = "v1.0.1";
    private Version _version;
    /// <summary>
    /// Mayor Minor and Patch Version is stored in this Model. 
    /// </summary>
    public Version Version
    {
        get => _version;
        set
        {
            _versionString = value.ToString(); 
            _version = value;
        }
    }

    private string _versionString = string.Empty;
    /// <summary>
    /// The String representation of the Version. => Output of Version.ToString()
    /// Note: Will be update when the Version is changed.
    /// </summary>
    public string VersionString
    {
        get => _versionString;
        set
        {
            _versionString = value; 
            _version = Version.Parse(_versionString);
        }
    }

    /// <summary>
    /// A markdown-based Description. This is the same description as in GitHub.
    /// </summary>
    public string MarkdownDescription { get; set; } = string.Empty;
    /// <summary>
    /// The date and time when this Version was published
    /// </summary>
    public DateTime Published { get; set; } = DateTime.Now;
    /// <summary>
    /// When it is true, this version is a Long-Term Support version.
    /// </summary>
    public bool IsLTS { get; set; } = false;
    /// <summary>
    /// Is true when this Version is a pre Release before the master release.
    /// </summary>
    public bool IsPrerelease { get; set; } = false;
    private bool _isBeta = false;
    /// <summary>
    /// The second type of prerelease, which will be published after a Alpha Release.
    /// </summary>
    public bool IsBeta { 
        get => _isBeta;
        set { _isBeta = value; IsPrerelease = IsAlpha || IsBeta; }
    }
    private bool _isAlpha = false;
    /// <summary>
    /// Part of the group of prereleases. The alpha version will be released first.
    /// </summary>
    public bool IsAlpha
    {
        get => _isAlpha;
        set { _isAlpha = value; IsPrerelease = IsAlpha || IsBeta; } 
    } 
  /*  /// <summary>
    /// All people that have worked on this Version.
    /// </summary>
    public List<User> Authors { get; set; } = new List<User>();
    /// <summary>
    /// All Dependencies of a Product of a specific Version.
    /// </summary>
    public List<CompatibleToProduct> Dependencies { get; set; } = new List<CompatibleToProduct>();*/

    public VersionInfo()
    {
        Version = new Version();
    }

    public VersionInfo(Version version)
    {
        Version = version;
    }
    public bool IsHigherThanThis([NotNull]VersionInfo other)
    {
        if (other.Version.CompareTo(Version) == 1) return true;
        if(Version.Equals(other.Version))
        {
            if(IsPrerelease && !other.IsPrerelease) return true;
            if(IsAlpha && other.IsBeta) return true;
        }
        return false;
    }
    public bool IsLowerThanThis([NotNull]VersionInfo other)
    {
        if (other.Version.CompareTo(Version) == -1) return true;
        if(Version.Equals(other.Version))
        {
            if(!IsPrerelease && other.IsPrerelease) return true;
            if(other.IsAlpha && IsBeta) return true;
        }
        return false;
    }
    public override bool Equals(object obj)
    {
        if (obj is VersionInfo versionInfo)
        {
            return versionInfo.Version.Equals(Version) && 
                   versionInfo.IsBeta == IsBeta && 
                   versionInfo.IsAlpha == IsAlpha && 
                   versionInfo.IsPrerelease == IsPrerelease && 
                   versionInfo.IsLTS == IsLTS && 
                   versionInfo.Published.Equals(Published);
        }
        return false;
    }

    public override string ToString()
    {
        return GetVersionString();
    }

    /// <summary>
    /// Creates a Version String in this Format: Mayor.Minor.Patch.Build.{Alpha or Beta}.{LTS if enabled}-(dd.MM.yyyy_HH:mm:ss)
    /// </summary>
    /// <returns></returns>
    public string GetVersionString()
    {
        int fieldCount = 3; //major.minor.build
        if(Version.Revision != -1) fieldCount = 4; //major.minor.build.revision
        
        if (IsAlpha)
            return Version.ToString(fieldCount) + ".Alpha-(" + Published.ToString("dd.MM.yyyy_HH:mm:ss") + ")";
        if (IsAlpha && IsLTS)
            return Version.ToString(fieldCount) + ".Alpha.LTS-(" + Published.ToString("dd.MM.yyyy_HH:mm:ss") + ")";
        if (IsBeta)
            return Version.ToString(fieldCount) + ".Beta-(" + Published.ToString("dd.MM.yyyy_HH:mm:ss") + ")";
        if (IsBeta && IsLTS)
            return Version.ToString(fieldCount) + ".Beta.LTS-(" + Published.ToString("dd.MM.yyyy_HH:mm:ss") + ")";
        if(IsLTS)
            return Version.ToString(fieldCount) + ".LTS-(" + Published.ToString("dd.MM.yyyy_HH:mm:ss") + ")";
        return Version.ToString(fieldCount)+ "-(" + Published.ToString("dd.MM.yyyy_HH:mm:ss") + ")";
    }

    public static VersionInfo? Parse(string versionString)
    {
        VersionInfo result = new VersionInfo();
        string completeVersionString = versionString.Split('-')[0];
        string publishedString = versionString.Split('-')[1].Replace("(", "").Replace(")", "");
        result.Published = DateTime.ParseExact(publishedString, "dd.MM.yyyy_HH:mm:ss", null);
        ExtractVersionNumbersAndLabelFromString(completeVersionString, result);
        return result;
    }

    private static void ExtractVersionNumbersAndLabelFromString(string completeVersionString, VersionInfo result)
    {
        int[] versionNumbers = new int[4]; // Max 4 Version Numbers Mayor.Minor.Patch(Issue_ID).LTS_Patch 
        int currentNumberIndex = 0;
        bool isAlpha = false;
        bool isBeta = false;
        bool isLTS = false;
        foreach (string part in completeVersionString.Split('.'))
        {
            if (int.TryParse(part, out int versionNumber))
            {
                versionNumbers[currentNumberIndex] = versionNumber;
                currentNumberIndex++;
            }
            else if(part == "Alpha")
            {
                result.IsAlpha = true;
            } 
            else if (part == "Beta")
            {
                result.IsBeta = true;
            }
            else if (part == "LTS")
            {
                result.IsLTS = true;
            }
            else
            {
                throw new ArgumentException("Invalid version string: " + part + "! You must follow this format ");
            }
        }
        CheckAndSetVersionNumbers(versionNumbers, result);
    }

    private static void CheckAndSetVersionNumbers(int[] versionNumbers, VersionInfo result)
    {
        int countOfDeclaredVersionNumbers = -1; // Must begin by -1 because the array start index is 0.
        for(int i =  0; i < 3; i++) if(versionNumbers[i] >= 0) countOfDeclaredVersionNumbers++;
        switch (countOfDeclaredVersionNumbers)
        {
            case 3:
                result.Version = new Version(versionNumbers[0], versionNumbers[1], versionNumbers[2], versionNumbers[3]);
                return;
            case 2:
                result.Version = new Version(versionNumbers[0], versionNumbers[1], versionNumbers[2]);
                return;
        }
        if(countOfDeclaredVersionNumbers < 2)
            throw new ArgumentException("There are to less Version Numbers in the Version String. Current count of numbers: " +  countOfDeclaredVersionNumbers);
        throw new ArgumentException("There are too many Version Numbers in the Version String. Current count of numbers: " +  countOfDeclaredVersionNumbers);
    }
}