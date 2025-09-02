using BoTech.StarClock.Api.Share;
using BoTech.StarClock.Api.SharedModels.ImageSlideshow;
using BoTech.StarClock.Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BoTech.StarClock.Api.Controllers;
/// <summary>
/// Controls the set of images, that are stored on the StarClock and which should be displayed.
/// </summary>

[ApiController]
[Route("[controller]")]
public class ImageSlideshowController : ControllerBase
{
    private static string CurrentImageIndexFileName { get; } =
        SystemPaths.GetBaseProgramDataPath("Images/Slideshow/") + "images.json";
    private static string CurrentSlideshowIndexFileName { get; } =
        SystemPaths.GetBaseProgramDataPath("Images/Slideshow/") + "slideshow.json";
    /// <summary>
    /// The Images that are stored on the device => Id, Path
    /// </summary>
    private ImageList _storedImages = new ImageList();
    private Slideshow _slideshow = new Slideshow();

    public ImageSlideshowController()
    {
        LoadJsonPropertiesFormFile(this);
    }

    public static Slideshow GetSlideshow()
    {
        if (System.IO.File.Exists(CurrentSlideshowIndexFileName))
        {
             return JsonConvert.DeserializeObject<Slideshow>(System.IO.File.ReadAllText(CurrentSlideshowIndexFileName));
        }
        return null;
    }
    /// <summary>
    /// Gets all images of the slideshow container
    /// </summary>
    /// <returns>ok</returns>
    [HttpGet]
    [Route("GetSlideshowImages")]
    public ActionResult<List<LocalImage>> GetSlideshowImages()
    {
        return Ok(_slideshow.Images.ToList());
    }
    /// <summary>
    /// Clears the Slideshow container.
    /// </summary>
    /// <returns>ok</returns>
    [HttpPut]
    [Route("ClearSlideshow")]
    public ActionResult ClearSlideshow()
    {
        _slideshow.Images.Clear();
        ApiStatusHandler.OnSlideShowChanged.Invoke(_slideshow);
        SaveChanges();
        return Ok("Cleared!");
    }
    /// <summary>
    /// Adds an image to the slide show at the end of the list.
    /// </summary>
    /// <param name="id">Image id that should be added</param>
    /// <returns>ok or notfound</returns>
    [HttpPatch]
    [Route("AddImageToSlideshow")]
    public ActionResult AddImageToSlideshow(string id)
    {
        LocalImage? localImage = _storedImages.Find(img => img.Id.ToString() == id);
        if (localImage != null)
        {
            _slideshow.Images.Add(localImage);
            ApiStatusHandler.OnSlideShowChanged.Invoke(_slideshow);
            SaveChanges();
            return Ok($"Added {localImage.Id}!");
        }
        return NotFound("Image not found!");
    }
    /// <summary>
    /// Adds a range of images to the list.
    /// </summary>
    /// <param name="ids">All ids of the Images to add.</param>
    /// <param name="index">The zero-based index at which the new Images should be inserted </param>
    /// <returns>ok or notfound</returns>
    [HttpPatch]
    [Route("AddImageRangeToSlideshow")]
    public ActionResult AddImageRangeToSlideshow(string[] ids, int index)
    {
        List<LocalImage> images = _storedImages.FindAll(img => ids.Contains(img.Id.ToString()));
        if (images.Count == ids.Length)
        {
            _slideshow.Images.InsertRange(index, images);
            ApiStatusHandler.OnSlideShowChanged.Invoke(_slideshow);
            SaveChanges();
            return Ok($"Added {images.Count} Images!");
        }
        return NotFound("Some or all Images not found!");
    }

    /// <summary>
    /// Saves a File to a local path and saves it to the <see cref="_storedImages"/> List.
    /// </summary>
    /// <param name="file"></param>
    /// <returns>The new id of the File.</returns>
    [HttpPost]
    [Route("UploadImage")]
    public async Task<ActionResult<string>> UploadImage(IFormFile file)
    {
        // Validate the file
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file was uploaded or the file is empty.");
        }
        try
        {
            LocalImage image = _storedImages.CreateAndAdd(file, SystemPaths.GetBaseProgramDataPath("Images/Slideshow/"));
            SaveChanges();
            // Save the file to the server
            using (var stream = new FileStream(image.FullName, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(image.Id);
        }
        catch (Exception ex)
        {
            // Handle errors
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    /// <summary>
    /// Deletes an Image in the List
    /// </summary>
    /// <param name="id">The id of the Image to delete.</param>
    /// <returns>NotFound or Ok</returns>
    [HttpDelete]
    [Route("DeleteImage")]
    public ActionResult DeleteImage(string id)
    {
        LocalImage? image = null;
        if ((image = _storedImages.Find(si => si.Id.ToString().Equals(id))) != null)
        {
             System.IO.File.Delete(image.FullName);
            _storedImages.Remove(image);
            SaveChanges();
            return Ok(image.Id);
        }
        return NotFound("Image not found.");
    }
    /// <summary>
    /// Returns all stored images
    /// </summary>
    /// <returns>all local images</returns>
    [HttpGet]
    [Route("GetImageList")]
    public ActionResult<List<LocalImage>> GetImageList()
    {
        return _storedImages.ToList();
    }
    /// <summary>
    /// Saves all Properties to a .json file.
    /// </summary>
    private void SaveChanges()
    {
        SaveObjectToFile(_storedImages, CurrentImageIndexFileName);
        SaveObjectToFile(_slideshow, CurrentSlideshowIndexFileName);
    }
    /// <summary>
    /// Saves any object to .json file.
    /// </summary>
    /// <param name="obj">The Object</param>
    /// <param name="fileName">The Filename including .json</param>
    private void SaveObjectToFile(object obj, string fileName)
    {
        string json = JsonConvert.SerializeObject(obj);
        if(!System.IO.File.Exists(fileName)) 
            System.IO.File.Create(fileName).Dispose(); // Dispose to detach the system from the file
        System.IO.File.WriteAllText(fileName, json);
    }
    private static void LoadJsonPropertiesFormFile(ImageSlideshowController controller)
    {
        if (System.IO.File.Exists(CurrentImageIndexFileName))
        {
            ImageList? imageList = JsonConvert.DeserializeObject<ImageList>(System.IO.File.ReadAllText(CurrentImageIndexFileName));
            if(imageList != null) controller._storedImages = imageList;
        }
        if (System.IO.File.Exists(CurrentSlideshowIndexFileName))
        {
            Slideshow? slideshow = JsonConvert.DeserializeObject<Slideshow>(System.IO.File.ReadAllText(CurrentSlideshowIndexFileName));
            if(slideshow != null) controller._slideshow = slideshow;
        }
    }
    /// <summary>
    /// A list with some custom functionality.
    /// </summary>
    private class ImageList : List<LocalImage>
    {
        /// <summary>
        /// Creates an instance of a LocalImage and saves it to the List.
        /// This method also ensures that the ID is unique.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns>The new LocalImage</returns>
        public LocalImage CreateAndAdd(IFormFile file, string path)
        {
            LocalImage img = new LocalImage(){
                Id = CreateNewGuid(),
                Path = path, 
                FileName = file.FileName
            };
            this.Add(img);
            return img;
        }
        /// <summary>
        /// Creates a new Guid that is not available in the list
        /// </summary>
        /// <returns>A real unique identifier.</returns>
        private Guid CreateNewGuid()
        {
            Guid guid = Guid.NewGuid();
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Id.Equals(guid))
                {
                    return CreateNewGuid();
                }
            }
            return guid;
        }
    }
}