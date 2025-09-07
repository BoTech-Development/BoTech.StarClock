using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using BoTech.StarClock.Helper;

namespace BoTech.StarClock.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.Loaded += OnLoaded;
        // Show the Window in full screen
        // More Info for Window definition here: https://github.com/AvaloniaUI/Avalonia/issues/2759 and https://forums.raspberrypi.com/viewtopic.php?t=358654
        if (OperatingSystem.IsLinux())
        {
            this.MaxHeight = 800;
            this.MaxWidth = 480;
            this.Height = 800;
            this.Width = 480;
            this.CanResize = false;
            this.SystemDecorations = SystemDecorations.None;
            this.WindowState = WindowState.Maximized;
            this.Topmost = true;
        }
    }
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // Start Backend api asynchronously
        Thread apiThread = new Thread(() =>
        {
            BoTech.StarClock.Api.Program.Main(UpdateManager.ThisVersion.GetVersionString());
        });
        apiThread.Start();
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        WindowNotificationManagerHandler.Init(this);
    }
}