using System;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using BoTech.StarClock.Views;

namespace BoTech.StarClock.Helper;

public static class WindowNotificationManagerHandler
{
    public static WindowNotificationManager NotificationManager { get; set; }
    public static void Init(MainWindow mainWindow)
    {
        var topLevel = TopLevel.GetTopLevel(mainWindow);
        NotificationManager = new WindowNotificationManager(topLevel) { MaxItems = 3 };
    }


}