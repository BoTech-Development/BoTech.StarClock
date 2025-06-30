using Avalonia.Controls;

namespace BoTech.StarClock.ViewModels.Standard.Form;

public class TextFormInput : FormInput
{
    private string _input = string.Empty;
    public override object GetResult() => _input;
    public TextFormInput (string name, string helpText, string watermark) : base(name, helpText)
    {
        InputControl = new TextBox()
        {
            Watermark = watermark,
        };
        ((TextBox)InputControl).TextChanged += (s, e) => _input = ((TextBox)InputControl).Text;
    }
}