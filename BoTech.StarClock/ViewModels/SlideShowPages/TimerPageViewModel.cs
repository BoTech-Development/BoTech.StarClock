using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Timers;
using BoTech.StarClock.ViewModels.Standard;
using BoTech.StarClock.ViewModels.Standard.Form;
using BoTech.StarClock.Views.Standard;
using DialogHostAvalonia;
using ReactiveUI;
using Ursa.Controls;

namespace BoTech.StarClock.ViewModels.SlideShowPages;

public class TimerPageViewModel : ViewModelBase
{
    public List<Timer> Timers { get; set; }
    private int _progressIndex = 0;
    public int ProgressIndex
    {
        get => _progressIndex;
        set => this.RaiseAndSetIfChanged(ref this._progressIndex, value);
    }
    private string _progressText;
    public string ProgressText
    {
        get => _progressText;
        set => this.RaiseAndSetIfChanged(ref _progressText, value);
    }
    /// <summary>
    /// Calls a function that shows the current time on the screen
    /// </summary>
    private System.Timers.Timer _timerUpdate;
    public TimerPageViewModel()
    {
        _timerUpdate = new System.Timers.Timer();
        _timerUpdate.Interval = 1000;
        _timerUpdate.Enabled = true;
        _timerUpdate.AutoReset = true;
        _timerUpdate.Elapsed += UpdateTimer;
        Timers = new List<Timer>()
        {
            new Timer(this,DateTime.Parse("2025-06-09 9:50"), DateTime.Parse("2025-06-09 12:20"), "Test1"),
            new Timer(this,DateTime.Parse("2013-04-21 13:26"), DateTime.Parse("2013-05-21 15:00"), "Test2")
            {
                IsActive = false,
            }
        };
    }
    /// <summary>
    /// Updates the Progress bar every second.
    /// ProgressBar show the Timer that will ring in the smallest time diff.
    /// </summary>
    /// <param name="sender">Not needed</param>
    /// <param name="e">Not needed</param>
    private void UpdateTimer(object? sender, ElapsedEventArgs e)
    {
        TimeSpan smallestDiff = TimeSpan.MaxValue;
        Timer? smallestDiffTimer = null;
        foreach (var timer in Timers)
        {
            if (timer.IsActive)
            {
                if (smallestDiff >= timer.GetCurrentDiff())
                {
                    smallestDiff = timer.GetCurrentDiff();
                    smallestDiffTimer = timer;
                }
            }
        }

        if (smallestDiffTimer != null)
        {
           
            double sum = (smallestDiffTimer.End - smallestDiffTimer.Start).TotalSeconds;
            ProgressIndex = (int)(100 - (smallestDiff.TotalSeconds / sum) * 100); // Mal 100 da komma zahl nicht geht und 100- blabla da wir runterzählen
            ProgressText = smallestDiff.ToString(@"hh\:mm\:ss");
        }
    }
    public class Timer
    {
        public string Name { get; set; }
        private DateTime _start;
        public DateTime Start
        {
            get => _start;
            set
            {
                _start = value;
                Duration = End - Start; 
            }
        }
        private DateTime _end;
        public DateTime End
        {
            get => _end;
            set
            {
                _end = value;
                Duration = End - Start; 
            }
        }
        public TimeSpan Duration { get; set; }
        public bool IsActive { get; set; } = true;
        public bool SoundEnabled { get; set; } = true;
        
        public ReactiveCommand<Unit, Unit> EditCommand { get; set; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; set; }
        
        private TimerPageViewModel _mainViewModel;
        public Timer(TimerPageViewModel viewModel, DateTime start, DateTime end, string name)
        {
            Start = start;
            End = end;
            Name = name;
            _mainViewModel = viewModel;
            EditCommand = ReactiveCommand.Create(Edit);
            DeleteCommand = ReactiveCommand.Create(Delete);
        }
        
        public TimeSpan GetCurrentDiff()
        {
            return End - DateTime.Now;
        }

        private async void Edit()
        {
            List<FormInput> inputs = new List<FormInput>();
            inputs.Add(new TextFormInput("Name", "The name of the timer.", "Enter the name of the Timer."));
            inputs.Add(new TextFormInput("Name1", "The name of the timer.", "Enter the name of the Timer."));
            inputs.Add(new TextFormInput("Name2", "The name of the timer.", "Enter the name of the Timer."));
            inputs.Add(new TextFormInput("Name3", "The name of the timer.", "Enter the name of the Timer."));
            inputs.Add(new TextFormInput("Name4", "The name of the timer.", "Enter the name of the Timer."));
            inputs.Add(new TextFormInput("Name5", "The name of the timer.", "Enter the name of the Timer."));
            inputs.Add(new TextFormInput("Name6", "The name of the timer.", "Enter the name of the Timer."));
            inputs.Add(new TextFormInput("Name7", "The name of the timer.", "Enter the name of the Timer."));
            inputs.Add(new TextFormInput("Name8", "The name of the timer.", "Enter the name of the Timer."));
            FormBuilderViewModel formBuilder = new FormBuilderViewModel("Edit or create Timer: " + Name, inputs);
            DialogHost.Show(new FormBuilderView()
            {
                DataContext = formBuilder
            },"MainDialogHost");
          //  FormResult result = await formBuilder.Show();
        }

        private void Delete()
        {
            _mainViewModel.Timers.Remove(this);
        }
    }
}