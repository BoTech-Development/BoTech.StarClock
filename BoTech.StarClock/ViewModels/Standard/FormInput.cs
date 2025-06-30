using Avalonia.Controls;

namespace BoTech.StarClock.ViewModels.Standard;

public abstract class FormInput (string name, string helpText)
{
    public string Name { get; set; } = name;
    public string HelpText { get; set; } = helpText;
    public Control InputControl { get; internal set; }
    public abstract object GetResult();
}
