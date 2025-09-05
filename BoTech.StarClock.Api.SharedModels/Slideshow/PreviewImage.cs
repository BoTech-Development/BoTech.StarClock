
using Avalonia.Media.Imaging;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using Size = SixLabors.ImageSharp.Size;

namespace BoTech.StarClock.Api.SharedModels.Slideshow;
/// <summary>
/// A downscaled version of a LocalImage.
/// </summary>
public class PreviewImage
{
    private string _fileName;
    public PreviewImage(string pathToImage, string savePath, string imageFileName, int width, int height)
    {
        ResizeImage(pathToImage, savePath, imageFileName, width, height);
    }

    public void DeleteFile()
    {
        if(File.Exists(_fileName)) File.Delete(_fileName);
    }
    public byte[]? ReadAllBytes()
    {
        if (File.Exists(_fileName))
        {
            return File.ReadAllBytes(_fileName);
        }
        return null;
    }
    private void ResizeImage(string pathToImage, string savePath, string imageFileName, int width, int height)
    {
        if (File.Exists(pathToImage))
        {
            /*using (FileStream input = File.OpenRead(pathToImage))
            using (SKBitmap original = SKBitmap.Decode(input))
            using (SKBitmap resized = original.Resize(new SKImageInfo(size.Width, size.Height), SKFilterQuality.High))
                return resized.ToBitmap();*/
            using (Image image = Image.Load(pathToImage))
            {
                image.Mutate(x => x.Resize(width, height));
                _fileName = savePath + imageFileName + ".jpeg";
                Stream fileStream = File.Create(savePath + imageFileName + ".jpeg");
                image.Save(fileStream, new JpegEncoder { Quality = 80 });
                fileStream.Close();
                fileStream.Dispose();
                return;
            }
        }
        throw new FileNotFoundException(pathToImage);
    }
}