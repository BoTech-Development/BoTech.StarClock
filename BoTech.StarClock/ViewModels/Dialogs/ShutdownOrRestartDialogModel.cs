using System;
using System.Diagnostics;
using System.Reactive;
using ReactiveUI;

namespace BoTech.StarClock.ViewModels.Dialogs;

public class ShutdownOrRestartDialogModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> RestartCommand { get; }
    public ReactiveCommand<Unit, Unit> ShutdownCommand { get; }

    public ShutdownOrRestartDialogModel()
    {
        RestartCommand = ReactiveCommand.Create(SystemControl.Restart);
        ShutdownCommand = ReactiveCommand.Create(SystemControl.ShutDown);
    }
    private static class SystemControl
    {
        /// <summary>
        /// Windows or Linux restart
        /// </summary>
        public static void Restart()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                StartCommand("cmd","/C shutdown -f -r -t 5");
            }
            else if(Environment.OSVersion.Platform == PlatformID.Unix)
            {
                StartCommand("bash","sudo reboot now");
            }  
            
        }

        /// <summary>
        /// Log off.
        /// </summary>
        public static void LogOff()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                StartCommand("cmd","/C shutdown -l");
            }
            else if(Environment.OSVersion.Platform == PlatformID.Unix)
            {
                throw new NotImplementedException();
            }  
        }

        /// <summary>
        ///  Shutting Down Windows or Linux
        /// </summary>
        public static void ShutDown()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                StartCommand( "cmd","/C shutdown -f -s -t 5");
            }
            else if(Environment.OSVersion.Platform == PlatformID.Unix)
            {
                StartCommand("bash","sudo shutdown now");
            }  
        }

        private static void StartCommand(string filename, string param)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = filename;
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Arguments = param;
            Process.Start(proc);
        }
    }
}