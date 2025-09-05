using BoTech.StarClock.Api.SharedModels.Slideshow;

namespace BoTech.StarClock.Api.Share;

/// <summary>
/// When the User changes something at the slideshow
/// </summary>
public delegate void OnSlideShowChanged(Slideshow newSlideshow);
/// <summary>
/// Contains all events that can be invoked by the api to notify the UI, that something had changed.
/// </summary>
public class ApiStatusHandler
{
    public static OnSlideShowChanged OnSlideShowChanged;
}