

using System;
using System.IO;
using System.Net.Http;

namespace BoTech.StarClock.Helper;

public delegate void OnFileProgressChanged(DownloadProgressChangedEventArgs e);
public class FileDownloader
{
    public event OnFileProgressChanged FileProgressChanged;
    /// <summary>
    /// Downloads a file from a specific url. In addition, it invokes the <see cref="FileProgressChanged"/>.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="destinationPath"></param>
    public bool DownloadFileWithProgress(string url, string destinationPath, string progressText = "")
    {
        try
        {
            DownloadFileWithProgressUnsafe(url, destinationPath, progressText);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private void DownloadFileWithProgressUnsafe(string url, string destinationPath, string progressText)
    {
        using (HttpClient client = new HttpClient())
        using (HttpResponseMessage response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult())
        using (Stream stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
        using (FileStream fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            response.EnsureSuccessStatusCode();

            long totalBytes = response.Content.Headers.ContentLength ?? -1;
            byte[] buffer = new byte[8192];
            int bytesRead;
            long totalRead = 0;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, bytesRead);
                totalRead += bytesRead;

                if (totalBytes > 0)
                {
                    if (progressText == "")
                    {
                        progressText = Path.GetFileName(destinationPath);
                    }
                    FileProgressChanged.Invoke(new DownloadProgressChangedEventArgs()
                    {
                        Percentage = (int)(((double) totalRead / totalBytes) * 100), 
                        BytesReceived = totalRead,
                        TotalBytesToReceive = totalBytes,
                        ProgressText = progressText
                    });
                    /*  long remaining = totalBytes - totalRead;
                    //  Console.WriteLine($"Downloaded: {totalRead} bytes, Remaining: {remaining} bytes");
                    Console.WriteLine("Downloaded {0}/{1}", totalRead, totalBytes);*/
                }
                else
                {
                    Console.WriteLine($"Downloaded: {totalRead} bytes");
                }
            }
        }
    }
}
public class DownloadProgressChangedEventArgs : EventArgs
{
    public int Percentage { get; set; }
    public long BytesReceived { get; set; }
    public long TotalBytesToReceive { get; set; }
    public long BytesRemaining { get { return TotalBytesToReceive - BytesReceived; } }
    /// <summary>
    /// Will be added to the Progress info Text.
    /// </summary>
    public string ProgressText { get; set; }
}