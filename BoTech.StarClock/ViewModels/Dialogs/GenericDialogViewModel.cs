using System.Reactive;
using Avalonia.Controls;
using Avalonia.Media;
using DialogHostAvalonia;
using Material.Icons;
using Material.Icons.Avalonia;
using ReactiveUI;

namespace BoTech.StarClock.ViewModels.Dialogs;

public class GenericDialogViewModel : ViewModelBase
{
    private Control _content;

    public Control Content
    {
        get => _content; 
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }
    private MaterialIconKind _icon;

    public MaterialIconKind Icon
    {
        get => _icon; 
        set => this.RaiseAndSetIfChanged(ref _icon, value);
    }

    private IBrush _iconColor;
    public IBrush IconColor
    {
        get => _iconColor; 
        set =>  this.RaiseAndSetIfChanged(ref _iconColor, value);
    }
    private bool _isHelpOptionEnabled;
    public bool IsHelpOptionEnabled
    {
        get => _isHelpOptionEnabled;
        set =>  this.RaiseAndSetIfChanged(ref _isHelpOptionEnabled, value);
    }
    private bool _isSaveOptionEnabled;
    public bool IsSaveOptionEnabled 
    {
        get => _isSaveOptionEnabled;
        set => this.RaiseAndSetIfChanged(ref _isSaveOptionEnabled, value); 
    }
    private bool _isCloseOptionEnabled;
    public bool IsCloseOptionEnabled
    {
        get => _isCloseOptionEnabled;
        set => this.RaiseAndSetIfChanged(ref _isCloseOptionEnabled, value);
    }
    public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }
    public ReactiveCommand<Unit, Unit> HelpCommand { get; set; }

    public GenericDialogViewModel()
    {
        CloseCommand = ReactiveCommand.Create(() =>
        {
            DialogHost.Close("MainDialogHost");
        });
        
    }
}