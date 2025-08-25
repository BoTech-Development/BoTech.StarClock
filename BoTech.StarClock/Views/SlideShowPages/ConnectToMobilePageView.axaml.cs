using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BoTech.StarClock.Views.Abstraction;

namespace BoTech.StarClock.Views.SlideShowPages;

public partial class ConnectToMobilePageView : UserControl, ISlideShowPage
{
    public string Name { get; private set; } = "ConnectToMobile";
    public ConnectToMobilePageView()
    {
        InitializeComponent();
    }
}