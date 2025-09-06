using System;
using System.Diagnostics;
using System.Reactive;
using System.Threading;
using Avalonia.Media;
using BoTech.StarClock.Helper;
using Material.Icons;
using ReactiveUI;

namespace BoTech.StarClock.ViewModels.Dialogs;

public class UpdateOsOrAppDialogModel : ViewModelBase
{
    private int _currentProgress;
    /// <summary>
    /// Current Percentage of the Progress Bar
    /// </summary>
    public int CurrentProgress
    {
        get =>  _currentProgress;
        set => this.RaiseAndSetIfChanged(ref _currentProgress, value);
    }
    private string _currentStatus = String.Empty;
    /// <summary>
    /// The Content of the label above the progress bar
    /// </summary>
    public string CurrentStatus
    {
        get => _currentStatus;
        set => this.RaiseAndSetIfChanged(ref _currentStatus, value);
    }
    private bool _isProgressBarIndeterminate = false;
    /// <summary>
    /// Animated Progress or based on percentage
    /// </summary>
    public bool IsProgressBarIndeterminate
    {
        get => _isProgressBarIndeterminate;
        set => this.RaiseAndSetIfChanged(ref _isProgressBarIndeterminate, value);
    }
    private bool _isProgressVisible = false;
    /// <summary>
    /// Shows the two buttons or the Progress bar.
    /// </summary>
    public bool IsProgressVisible 
    { 
        get => _isProgressVisible; 
        set => this.RaiseAndSetIfChanged(ref _isProgressVisible, value); 
    }


    public ReactiveCommand<Unit, Unit> UpdateAppCommand { get; }
    public ReactiveCommand<Unit, Unit> UpdateOsCommand { get; }
    
    public GenericDialogViewModel DialogViewModel { get; set; }
    public UpdateOsOrAppDialogModel(GenericDialogViewModel dialogViewModel)
    {
        DialogViewModel = dialogViewModel;
        UpdateManager um = new UpdateManager();
        um.OnCurrentStatusChanged += OnOnCurrentStatusChanged;
       // um.DeleteOldInstalledVersions();
        //CheckForUpdatesCommand = ReactiveCommand.Create(CheckForUpdates);
        // UpdateNowNotificationCommand = ReactiveCommand.Create(() => {_updateManager.InstallUpdates();});
        UpdateAppCommand = ReactiveCommand.Create(() =>
        {
            IsProgressVisible = true;
            Thread updateThread = new Thread(() =>
            {
                if (um.CheckForUpdate())
                {
                    if (!um.IsNextUpdateUnstable)
                    {
                        if (!um.InstallUpdate())
                        {
                            CurrentStatus = "Error by downloading or installing update!";
                            IsProgressBarIndeterminate = false;
                            CurrentProgress = 100;
                            DialogViewModel.Icon = MaterialIconKind.AlertCircleOutline;
                            DialogViewModel.IconColor = Brushes.Red;
                        }
                        else
                        {
                            CurrentStatus = "Please restart to activate the update. Click ok to restart or cancel to do it later.";
                            IsProgressBarIndeterminate = false;
                            CurrentProgress = 100;
                            DialogViewModel.Icon = MaterialIconKind.CheckCircleOutline;
                            DialogViewModel.IconColor = Brushes.Green;
                        }
                    }
                    else
                    {
                        // TODO: Ask the user if he wants to perform the update
                        if (!um.InstallUpdate())
                        {
                            CurrentStatus = "Error by downloading or installing update!";
                            IsProgressBarIndeterminate = false;
                            CurrentProgress = 100;
                            DialogViewModel.Icon = MaterialIconKind.AlertCircleOutline;
                            DialogViewModel.IconColor = Brushes.Red;
                        }
                        else
                        {
                            CurrentStatus = "Please restart to activate the update. Click ok to restart or cancel to do it later.";
                            IsProgressBarIndeterminate = false;
                            CurrentProgress = 100;
                            DialogViewModel.Icon = MaterialIconKind.CheckCircleOutline;
                            DialogViewModel.IconColor = Brushes.Green;
                        }
                        
                    }
                }
                else
                {
                    CurrentStatus = "No update available!";
                    IsProgressBarIndeterminate = false;
                    CurrentProgress = 100;
                    DialogViewModel.Icon = MaterialIconKind.CheckCircleOutline;
                    DialogViewModel.IconColor = Brushes.Green;
                }
            });
            updateThread.Start();
        });
        UpdateOsCommand = ReactiveCommand.Create(() =>
        {
            SystemUpdater.UpdateLinux(this);
        });
    }
    private void OnOnCurrentStatusChanged(UpdateManager updaterInstance, string currentStatus)
    {
        CurrentStatus = currentStatus;
        CurrentProgress = updaterInstance.CurrentProgress;
        IsProgressBarIndeterminate = updaterInstance.IsProgressBarIndeterminate;
    }
    private static class SystemUpdater
    {
        /// <summary>
        /// Upgrade all packages hosted under apt and removes all packages that became useless after the upgrade
        /// </summary>
        public static void UpdateLinux(UpdateOsOrAppDialogModel model)
        {
            model.IsProgressVisible = true;
            model.IsProgressBarIndeterminate = true;
            model.CurrentStatus = "Checking for updates...";
            
            TerminalRunner.StartTerminal("sudo apt update");
            
            
            model.CurrentStatus = "Installing updates...";
            Console.WriteLine("Finished Checking for Updates...");
            
           TerminalRunner.StartTerminal("sudo apt upgrade -y");


            model.CurrentStatus = "Removing useless packages...";
            Console.WriteLine("Finished upgrading packages...");
            
             TerminalRunner.StartTerminal("sudo apt autoremove -y");

            
            Console.WriteLine("Finished autoremove...");
            
            model.CurrentStatus = "Update(s) installed when there any updates are available.";
            model.IsProgressBarIndeterminate = false;
            model.CurrentProgress = 100;
            model.DialogViewModel.Icon = MaterialIconKind.CheckCircleOutline;
            model.DialogViewModel.IconColor = Brushes.Green;
        }
    }
}