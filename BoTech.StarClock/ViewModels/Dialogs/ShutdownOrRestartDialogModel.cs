using System;
using System.Diagnostics;
using System.Reactive;
using BoTech.StarClock.Helper;
using ReactiveUI;

namespace BoTech.StarClock.ViewModels.Dialogs;

public class ShutdownOrRestartDialogModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> RestartCommand { get; }
    public ReactiveCommand<Unit, Unit> ShutdownCommand { get; }

    public ShutdownOrRestartDialogModel()
    {
        RestartCommand = ReactiveCommand.Create(() =>
        {
            SystemControl.Restart(this);
        });
        ShutdownCommand = ReactiveCommand.Create(() =>
        {
            SystemControl.ShutDown(this);
        });
    }
    private static class SystemControl
    {
        /// <summary>
        /// Windows or Linux restart
        /// </summary>
        public static void Restart(ShutdownOrRestartDialogModel model)
        {
            
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // StartCommand("cmd","/C shutdown -f -r -t 5", model);
            }
            else if(Environment.OSVersion.Platform == PlatformID.Unix)
            {
                TerminalRunner.StartTerminal("sudo reboot now"); //StartCommand("bash","sudo reboot now", model);
            }  
            
        }

        /// <summary>
        /// Log off.
        /// </summary>
        public static void LogOff(ShutdownOrRestartDialogModel model)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
               // StartCommand("cmd","/C shutdown -l", model);
            }
            else if(Environment.OSVersion.Platform == PlatformID.Unix)
            {
                throw new NotImplementedException();
            }  
        }

        /// <summary>
        ///  Shutting Down Windows or Linux
        /// </summary>
        public static void ShutDown(ShutdownOrRestartDialogModel model)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                //StartCommand( "cmd","/C shutdown -f -s -t 5", model);
            }
            else if(Environment.OSVersion.Platform == PlatformID.Unix)
            {
                TerminalRunner.StartTerminal("sudo shutdown now"); 
            }  
        }
    }
}