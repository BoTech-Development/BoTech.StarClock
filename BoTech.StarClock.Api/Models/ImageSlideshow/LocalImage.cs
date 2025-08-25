namespace BoTech.StarClock.Api.Models.ImageSlideshow;

/// <summary>
/// A wrapper class which stores info about the stored Image.
/// </summary>
/// <param name="guid"></param>
/// <param name="path"></param>
/// <param name="fileName"></param>
public class LocalImage
{
    /// <summary>
    /// The unique id of the Image
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// The Path to the Image
    /// </summary>
    public string Path { get; set; }

    private string _fileName;
    /// <summary>
    /// Filename of the image which includes the unique id.
    /// </summary>
    public string FileName
    {
        get
        {
            if(_fileName.Contains(Id.ToString())) return _fileName;
            return $"{Id.ToString()}_{_fileName}";
        }
        set => _fileName = value;
    }

    /// <summary>
    /// Full Filepath and filename
    /// </summary>
    public string FullName => System.IO.Path.Combine(Path, FileName);
}