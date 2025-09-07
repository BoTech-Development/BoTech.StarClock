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
    /// <summary>
    /// The Version of this app.
    /// </summary>
    public string AppVersion {get; set;} = UpdateManager.ThisVersion.ToString();
    public ReactiveCommand<Unit, Unit> ShutdownOrRestartCommand { get; }
    public ReactiveCommand<Unit, Unit> CheckForUpdatesCommand { get; }
    public SlideShowViewModel()
    {
        UpdateManager um = new UpdateManager();
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
        Pages.Add(new ConnectToMobilePageView()
        {
            DataContext = new ConnectToMobilePageViewModel()
        });
        ShutdownOrRestartCommand = ReactiveCommand.Create(() => ShutdownOrRestart());
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