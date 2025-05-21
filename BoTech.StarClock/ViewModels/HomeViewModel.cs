using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using Avalonia.Media.Imaging;
using BoTech.StarClock.Helper;
using ReactiveUI;

namespace BoTech.StarClock.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private List<Bitmap> _displayedImages = new List<Bitmap>();

    public List<Bitmap> DisplayedImages
    {
        get => _displayedImages;
        set => this.RaiseAndSetIfChanged(ref _displayedImages, value);
    }
    public ReactiveCommand<Unit, Unit> CheckForUpdatesCommand { get; }
    public HomeViewModel()
    {
        CheckForUpdatesCommand = ReactiveCommand.Create(() => { });
      /*  bool loaded = false;
        int imageNumber = 1;
        while (imageNumber < 10)
        {
            //if(!File.Exists("C:\\Users\\fteet\\OneDrive\\Dokumente\\BoTech\\Projects\\StarClock\\StarClock\\Assets\\" + imageNumber + ".jpg")) {loaded = true;break;}
            Bitmap bitmap = ImageHelper.LoadFromResource(new Uri("avares://Assets/"+imageNumber+".jpg")); // new Bitmap("C:\\Users\\fteet\\OneDrive\\Dokumente\\BoTech\\Projects\\StarClock\\StarClock\\Assets\\" + imageNumber + ".jpg");
            DisplayedImages.Add(bitmap);
            imageNumber++;
        }*/
       
    }
}