using BoTech.StarClock.Api.SharedModels;
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
    
    [HttpPost]
    [Route("SetDeviceName")]
    public ActionResult<bool> SetDeviceName( [FromQuery] string deviceName)
    {
        _deviceInfo.DeviceName = deviceName;
        _deviceInfo.SaveChanges();
        return Ok(_deviceInfo.DeviceName);
    }
    [HttpGet("GetDeviceAppVersion")]
    public ActionResult<string> GetDeviceAppVersion() => Ok(Program.VersionString);

}