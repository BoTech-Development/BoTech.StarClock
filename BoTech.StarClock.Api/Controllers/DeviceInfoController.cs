using BoTech.StarClock.Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BoTech.StarClock.Api.Controllers;
/// <summary>
/// Contains info about the device. The most params are settable. 
/// </summary>
[ApiController]
[Route("[controller]")]
public class DeviceInfoController : ControllerBase
{
    private readonly DeviceInfo _deviceInfo;
    public DeviceInfoController()
    {
        _deviceInfo = DeviceInfo.FromJsonFile(SystemPaths.GetBaseProgramDataPath() + "DeviceInfo.json");
        // occurs when there is no .json file => first run of the app    
        if (_deviceInfo == null)
        {
            _deviceInfo = new DeviceInfo()
            {
                DeviceName = Guid.NewGuid().ToString(),
            };
        }
    }
    [HttpGet("GetDeviceName")]
    public ActionResult<string> GetDeviceName() => Ok(_deviceInfo.DeviceName);
    
    [HttpPost("SetDeviceName")]
    public ActionResult<bool> SetDeviceName(string deviceName)
    {
        _deviceInfo.DeviceName = deviceName;
        _deviceInfo.SaveChanges();
        return Ok(_deviceInfo.DeviceName);
    }

    private class DeviceInfo
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
            if(!System.IO.File.Exists(CurrentFileName)) 
                System.IO.File.Create(CurrentFileName).Dispose();
            System.IO.File.WriteAllText(CurrentFileName, json);
        }
    }
}