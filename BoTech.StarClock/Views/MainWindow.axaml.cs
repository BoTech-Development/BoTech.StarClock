using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using BoTech.StarClock.Helper;

namespace BoTech.StarClock.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        WindowNotificationManagerHandler.Init(this);
    }
}