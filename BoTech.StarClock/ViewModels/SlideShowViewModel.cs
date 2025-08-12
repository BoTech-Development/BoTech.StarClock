using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using BoTech.StarClock.Helper;
using BoTech.StarClock.ViewModels.Dialogs;
using BoTech.StarClock.ViewModels.SlideShowPages;
using BoTech.StarClock.Views.Abstraction;
using BoTech.StarClock.Views.Dialogs;
using BoTech.StarClock.Views.SlideShowPages;
using DialogHostAvalonia;
using Material.Icons;
using ReactiveUI;

namespace BoTech.StarClock.ViewModels;

public class SlideShowViewModel : ViewModelBase
{
    public List<ISlideShowPage> Pages { get; set; } = new List<ISlideShowPage>();
   
    private int _currentProgress;
    public int CurrentProgress
    {
        get =>  _currentProgress;
        set => this.RaiseAndSetIfChanged(ref _currentProgress, value);
    }
    private string _currentStatus;

    public string CurrentStatus
    {
        get => _currentStatus;
        set => this.RaiseAndSetIfChanged(ref _currentStatus, value);
    }
    private bool _isProgressBarIndeterminate = false;

    public bool IsProgressBarIndeterminate
    {
        get => _isProgressBarIndeterminate;
        set => this.RaiseAndSetIfChanged(ref _isProgressBarIndeterminate, value);
    }
    private bool _isUpdating = false;
    /// <summary>
    /// When true the updater is working on any update.
    /// </summary>
    public bool IsUpdating
    {
        get =>  _isUpdating; 
        set =>  this.RaiseAndSetIfChanged(ref _isUpdating, value);
    }
    
    public ReactiveCommand<Unit, Unit> ShutdownOrRestartCommand { get; }
    public ReactiveCommand<Unit, Unit> CheckForUpdatesCommand { get; }
    public SlideShowViewModel()
    {
        UpdateManager um = new UpdateManager();
        um.OnCurrentStatusChanged += OnOnCurrentStatusChanged;
        um.DeleteOldInstalledVersions();
        //CheckForUpdatesCommand = ReactiveCommand.Create(CheckForUpdates);
       // UpdateNowNotificationCommand = ReactiveCommand.Create(() => {_updateManager.InstallUpdates();});
        CheckForUpdatesCommand = ReactiveCommand.Create(() =>
        {
            GenericDialogViewModel vm = new GenericDialogViewModel()
            {
                IsCloseOptionEnabled = true,
                IconColor = Brushes.Orange,
                Icon = MaterialIconKind.Update,
                
            };
            vm.Content = new UpdateOsOrAppDialog()
            {
                DataContext = new UpdateOsOrAppDialogModel(vm)
            };
            DialogHost.Show(new GenericDialogView()
            {
                DataContext = vm
            });
        });
        Pages.Add(new ClockPageView()
        {
            DataContext = new ClockPageViewModel()
        });
        Pages.Add(new TimerPageView()
        {
            DataContext = new TimerPageViewModel()
        });
        ShutdownOrRestartCommand = ReactiveCommand.Create(() => ShutdownOrRestart());
    }

    private void OnOnCurrentStatusChanged(UpdateManager updaterInstance, string currentStatus)
    {
        CurrentStatus = currentStatus;
        CurrentProgress = updaterInstance.CurrentProgress;
        IsProgressBarIndeterminate = updaterInstance.IsProgressBarIndeterminate;
    }

    private void ShutdownOrRestart()
    {
        DialogHost.Show(new GenericDialogView()
        {
            DataContext = new GenericDialogViewModel()
            {
                IsCloseOptionEnabled = true,
                IconColor = Brushes.Orange,
                Icon = MaterialIconKind.Power,
                Content = new ShutdownOrRestartDialog()
                {
                    DataContext = new ShutdownOrRestartDialogModel()
                }
            }
        });
    }
}