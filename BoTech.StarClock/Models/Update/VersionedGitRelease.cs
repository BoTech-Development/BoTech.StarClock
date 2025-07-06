using Octokit;

namespace BoTech.StarClock.Models.Update;
/// <summary>
/// This class connects an VersionInfo Model with a GitRelease. Through this connection, it is easier to access the Version of a release and to compare it with other versions.
/// </summary>
public class VersionedGitRelease(VersionInfo versionInfo, Release release)
{
    public VersionInfo VersionInfo { get; set; } = versionInfo;
    public Release Release { get; set; } =  release;
}