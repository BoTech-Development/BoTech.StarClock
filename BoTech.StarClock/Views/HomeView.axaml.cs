using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace BoTech.StarClock.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }
    public void Next(object source, RoutedEventArgs args)
    {
        //slides.Next();
    }

    public void Previous(object source, RoutedEventArgs args) 
    {
        //slides.Previous();
    }
}