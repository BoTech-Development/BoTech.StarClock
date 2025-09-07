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