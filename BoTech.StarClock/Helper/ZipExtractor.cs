using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace BoTech.StarClock.Helper;

public class ZipExtractor
{
    private UpdateManager _updateManager;

    public ZipExtractor(UpdateManager updateManager)
    {
        _updateManager = updateManager;
    }
    /// <summary>
    /// Extracts any .zip file to a specific folder.
    /// </summary>
    /// <param name="zipPath">path to the zip file</param>
    /// <param name="extractPath">the destination folder</param>
    /// <param name="progressText">The string which should be shown.</param>
    public void ExtractZipWithProgress(string zipPath, string extractPath, string progressText)
    {
        using (ZipArchive archive = ZipFile.OpenRead(zipPath))
        {
            ExtractZipEntriesWithProgress(archive, extractPath, GetNameOfParentFolderOfAllEntries(archive.Entries.ToList()), progressText);
        }
    }
    /// <summary>
    /// This Method check wheter all entries have the same parent folder.
    /// </summary>
    /// <param name="entries"></param>
    /// <returns>It returns the name of the Parent folder or string.Empty when not all entries have the same parent folder.</returns>
    private string? GetNameOfParentFolderOfAllEntries(List<ZipArchiveEntry> entries)
    {
        int indexOfBackslash = entries[0].FullName.IndexOf('/');
        string currentParentFolder = string.Empty;
        if(indexOfBackslash > 0)
            currentParentFolder = entries[0].FullName.Substring(0, indexOfBackslash);// Just set the first parent folder as the most parent folder
        foreach (ZipArchiveEntry entry in entries)
        {
            indexOfBackslash = entry.FullName.IndexOf('/');
            if (indexOfBackslash > 0)
            {
                if (entry.FullName.Substring(0, indexOfBackslash) != currentParentFolder)
                {
                    return string.Empty;
                }
            }else if (currentParentFolder != string.Empty)
                return string.Empty;
        }
        return currentParentFolder;
    }

    private void ExtractZipEntriesWithProgress(ZipArchive archive, string extractPath, string? parentEntryFolderToRemove, string progressText)
    {
        int totalEntries = archive.Entries.Count;
        int processedEntries = 0;
        foreach (var entry in archive.Entries)
        {
            string destinationPath = Path.Combine(extractPath, entry.FullName);

            if (string.IsNullOrEmpty(entry.Name)) // Directory entries will be handled in the else part.
            {
               // Directory.CreateDirectory(destinationPath);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(extractPath + entry.FullName.Replace(parentEntryFolderToRemove + "/", "")));
                entry.ExtractToFile(extractPath + entry.FullName.Replace(parentEntryFolderToRemove + "/", ""), overwrite: true);
            }

            processedEntries++;
             _updateManager.CurrentProgress = (int)((double)processedEntries / totalEntries * 100);
             _updateManager.CurrentStatus = "Extracting " + progressText + " (" + processedEntries + "/" + totalEntries + ")";
        }
    }
    
}