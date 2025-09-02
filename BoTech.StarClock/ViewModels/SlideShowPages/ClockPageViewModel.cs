using System;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Media.Imaging;
using BoTech.StarClock.Api.Controllers;
using BoTech.StarClock.Api.Share;
using BoTech.StarClock.Api.SharedModels.ImageSlideshow;
using BoTech.StarClock.Helper;
using ReactiveUI;

namespace BoTech.StarClock.ViewModels.SlideShowPages;

public class ClockPageViewModel : ViewModelBase
{

    private Bitmap? _backgroundImage;
    /// <summary>
    /// Image behind the Clock Text
    /// </summary>
    public Bitmap? BackgroundImage
    {
        get => _backgroundImage;
        private set => this.RaiseAndSetIfChanged(ref _backgroundImage, value);
    }
    private string _time = "??:??:??";
    public string Time 
    { 
        get => _time;
        set => this.RaiseAndSetIfChanged(ref _time, value); 
    }
    private string _date = "??.??.????";

    public string Date
    {
        get => _date;
        set => this.RaiseAndSetIfChanged(ref _date, value);
    }
    /// <summary>
    /// Calls a function that shows the current time on the screen
    /// </summary>
    private System.Timers.Timer _clockUpdateTimer;
    /// <summary>
    /// Calls a function that shows the next slideshow Image on the screen
    /// </summary>
    private System.Timers.Timer _slideshowUpdateTimer;
    private int _currentSlideshowIndex = 0;
    /// <summary>
    /// Current Slideshow instance.
    /// </summary>
    private Slideshow _slideshow;
    public ClockPageViewModel()
    {
        _clockUpdateTimer = new System.Timers.Timer(1000);
        _clockUpdateTimer.Enabled = true;
        _clockUpdateTimer.AutoReset = true;
        _clockUpdateTimer.Elapsed += ClockUpdateTimer_Elapsed;
        
        _slideshowUpdateTimer = new System.Timers.Timer(10000);
        _slideshowUpdateTimer.Enabled = true;
        _slideshowUpdateTimer.AutoReset = true;
        _slideshowUpdateTimer.Elapsed += SlideshowUpdateTimerOnElapsed;
        ApiStatusHandler.OnSlideShowChanged += OnSlideShowChanged;
        _slideshow = ImageSlideshowController.GetSlideshow();
    }

    private void SlideshowUpdateTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_slideshow != null)
        {
            _currentSlideshowIndex++;
            if (_currentSlideshowIndex >= _slideshow.Images.Count) _currentSlideshowIndex = 0;
            BackgroundImage = ImageHelper.LoadFromDeviceStorage(_slideshow.Images[_currentSlideshowIndex].FullName);
        }
    }

    private void OnSlideShowChanged(Slideshow newSlideshow)
    {
        Console.WriteLine("OnSlideShowChanged");
        _currentSlideshowIndex = 0;
        _slideshowUpdateTimer.Stop();
        // Waiting for SlideshowUpdateTimerOnElapsed Method to finish
        Task.Delay(2000).Wait(); // TODO: No Wait statements
        _slideshow = newSlideshow;//may be unnecessary
        _slideshowUpdateTimer.Start();
    }

    private void ClockUpdateTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        Time = DateTime.Now.ToString("HH:mm:ss");
        Date = DateOnly.FromDateTime(DateTime.Now).ToLongDateString();
    }
}