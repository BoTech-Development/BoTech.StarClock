using Microsoft.AspNetCore.Mvc;

namespace BoTech.StarClock.Api.Controllers;
/// <summary>
/// Controller for testing connections
/// </summary>
[ApiController]
[Route("[controller]")]
public class StatusController : ControllerBase
{
    /// <summary>
    /// Can be used by any client to check if the api is up and running
    /// </summary>
    /// <returns>All_Ok if everything works fine.</returns>
    [HttpGet]
    [Route("TestConnection")]
    public ActionResult<string> TestConnection() => Ok("All_Ok");
}