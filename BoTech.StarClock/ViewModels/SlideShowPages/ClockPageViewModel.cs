using System;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace BoTech.StarClock.ViewModels.SlideShowPages;

public class ClockPageViewModel : ViewModelBase
{
    /// <summary>
    /// Calls a function that shows the current time on the screen
    /// </summary>
    private System.Timers.Timer _clockUpdateTimer;
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
    public ClockPageViewModel()
    {
        _clockUpdateTimer = new System.Timers.Timer(1000);
        _clockUpdateTimer.Enabled = true;
        _clockUpdateTimer.AutoReset = true;
        _clockUpdateTimer.Elapsed += ClockUpdateTimer_Elapsed;
    }

    private void ClockUpdateTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        Time = DateTime.Now.ToString("HH:mm:ss");
        Date = DateOnly.FromDateTime(DateTime.Now).ToLongDateString();
    }
}