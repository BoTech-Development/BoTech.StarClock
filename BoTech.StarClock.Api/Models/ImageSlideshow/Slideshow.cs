using System.Net.Mime;
using BoTech.StarClock.Api.Controllers;

namespace BoTech.StarClock.Api.Models.ImageSlideshow;

public class Slideshow
{
    public List<LocalImage> Images { get; set; } = new List<LocalImage>();
}