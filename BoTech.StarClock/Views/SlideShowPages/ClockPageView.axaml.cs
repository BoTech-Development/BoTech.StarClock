using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BoTech.StarClock.Views.Abstraction;

namespace BoTech.StarClock.Views.SlideShowPages;

public partial class ClockPageView : UserControl, ISlideShowPage
{
    public string Name { get; private set; }

    public ClockPageView()
    {
        Name = "Clock-Page";
        InitializeComponent();
    }
}