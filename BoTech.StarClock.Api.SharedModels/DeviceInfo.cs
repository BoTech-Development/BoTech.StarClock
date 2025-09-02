using Newtonsoft.Json;

namespace BoTech.StarClock.Api.SharedModels;

public class DeviceInfo
{
    public string DeviceName { get; set; } = string.Empty;
        
    /// <summary>
    /// Current file path to save this object to.
    /// </summary>
    public static string CurrentFileName { get; private set; } = string.Empty;
    /// <summary>
    /// Loads this object from a specific file location.
    /// </summary>
    /// <param name="fileName">File path of the .json file.</param>
    /// <returns></returns>
    public static DeviceInfo? FromJsonFile(string fileName)
    {
        CurrentFileName = fileName;
        if (System.IO.File.Exists(fileName))
        {
            return JsonConvert.DeserializeObject<DeviceInfo>(System.IO.File.ReadAllText(fileName));
        }
        return null;
    }
    /// <summary>
    /// Saves this object to the specific File location in json format.
    /// </summary>
    public void SaveChanges()
    {
        string json = JsonConvert.SerializeObject(this);
        if(!File.Exists(CurrentFileName)) File.Create(CurrentFileName).Dispose();
        File.WriteAllText(CurrentFileName, json);
    }
}