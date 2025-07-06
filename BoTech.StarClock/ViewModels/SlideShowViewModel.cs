using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using BoTech.StarClock.Helper;
using BoTech.StarClock.ViewModels.SlideShowPages;
using BoTech.StarClock.Views.Abstraction;
using BoTech.StarClock.Views.SlideShowPages;
using ReactiveUI;

namespace BoTech.StarClock.ViewModels;

public class SlideShowViewModel : ViewModelBase
{
    public List<ISlideShowPage> Pages { get; set; } = new List<ISlideShowPage>();
    public ReactiveCommand<Unit, Unit> CheckForUpdatesCommand { get; }
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
    public ReactiveCommand<Unit, Unit> UpdateNowNotificationCommand { get; }
    public SlideShowViewModel()
    {
        UpdateManager um = new UpdateManager();
        um.OnCurrentStatusChanged += OnOnCurrentStatusChanged;
        um.DeleteOldInstalledVersions();
        //CheckForUpdatesCommand = ReactiveCommand.Create(CheckForUpdates);
       // UpdateNowNotificationCommand = ReactiveCommand.Create(() => {_updateManager.InstallUpdates();});
       CheckForUpdatesCommand = ReactiveCommand.Create(() =>
       {
           Thread updateThread = new Thread(() =>
           {
               UpdateManager um = new UpdateManager();
               um.OnCurrentStatusChanged += OnOnCurrentStatusChanged;
               if (um.CheckForUpdate())
               {
                   if (!um.IsNextUpdateUnstable)
                   {
                       if (!um.InstallUpdate())
                           CurrentStatus = "Error by downloading or installing update!";
                       else 
                           CurrentStatus = "Please restart to activate the update.";
                   }
                   else
                   {
                       // TODO: Ask the user if he wants to perform the update
                       if (!um.InstallUpdate())
                           CurrentStatus = "Error by downloading or installing update!";
                       else 
                           CurrentStatus = "Please restart to activate the update.";
                   }
               }
           });
           updateThread.Start();
       });
        Pages.Add(new ClockPageView()
        {
            DataContext = new ClockPageViewModel()
        });
        Pages.Add(new TimerPageView()
        {
            DataContext = new TimerPageViewModel()
        });
    }

    private void OnOnCurrentStatusChanged(UpdateManager updaterInstance, string currentStatus)
    {
        CurrentStatus = currentStatus;
        CurrentProgress = updaterInstance.CurrentProgress;
        IsProgressBarIndeterminate = updaterInstance.IsProgressBarIndeterminate;
    }

    /*    private void CheckForUpdates()
    {
        int countOfSystemUpdates = _updateManager.CheckForSystemUpdates();
        bool appNeedsUpdate = _updateManager.DoesThisAppNeedAnUpdate();
        if (appNeedsUpdate && countOfSystemUpdates > 0)
        {
            ShowUpdateNotification("App and System Updates", countOfSystemUpdates + 1, NotificationType.Warning);
        }
        else if(countOfSystemUpdates > 0)
        {
            ShowUpdateNotification("System Updates", countOfSystemUpdates, NotificationType.Information);
        }
        else if (appNeedsUpdate)
        {
            ShowUpdateNotification("App Updates", 1, NotificationType.Warning);
        }
    }
    /// <summary>
    /// Shows the update message. Show until the user closes it.
    /// </summary>
    /// <param name="updateKind"></param>
    /// <param name="countOfUpdates"></param>
    /// <param name="notificationType"></param>
    private void ShowUpdateNotification(string updateKind, int countOfUpdates, NotificationType notificationType)
    {
        WindowNotificationManagerHandler.NotificationManager.Show(new StackPanel()
        {
            Children =
            {
                new TextBlock()
                {
                    Text = updateKind + " available.",
                    FontWeight = FontWeight.Bold
                },
                new StackPanel()
                {
                    Children =
                    {
                        new TextBlock()
                        {
                            Text = "There are " + countOfUpdates + " " + updateKind +
                                   " available. Click here to install them."
                        },
                        new Button()
                        {
                            Content = "Install Updates",
                            Command = UpdateNowNotificationCommand,
                        }
                    }
                }

            }
        }, 
            notificationType, 
            new TimeSpan(0, 0, 0, 0, 0));
    }
    */
}