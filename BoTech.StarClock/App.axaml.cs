using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BoTech.StarClock.Helper;
using BoTech.StarClock.ViewModels;
using BoTech.StarClock.Views;

namespace BoTech.StarClock;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new SlideShowViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new SlideShowView()
            {
                DataContext = new SlideShowViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
        // Start Backend api asynchronously
        Thread apiThread = new Thread(() =>
        {
            BoTech.StarClock.Api.Program.Main(UpdateManager.ThisVersion.GetVersionString());
        });
        apiThread.Start();
    }
}