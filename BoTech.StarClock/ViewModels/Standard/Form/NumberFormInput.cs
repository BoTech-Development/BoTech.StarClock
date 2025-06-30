using Avalonia.Controls;

namespace BoTech.StarClock.ViewModels.Standard.Form;

public class NumberFormInput : FormInput
{
    private decimal? _input;
    public NumberFormInput(string name, string helpText, NumericUpDownConfiguration configuration) : base(name, helpText)
    {
        InputControl = new NumericUpDown()
        {
            Value = configuration.Value,
            Minimum = configuration.Minimum,
            Maximum = configuration.Maximum,
            Increment = configuration.Increment,
            FormatString = configuration.FormatString,
        };
        ((NumericUpDown)InputControl).ValueChanged += (s, e) => _input = ((NumericUpDown)InputControl).Value;
    }
    public override object GetResult() =>  _input;
}

public class NumericUpDownConfiguration
{
    public decimal? Value { get; set; }
    public decimal Increment { get; set; }
    public decimal Minimum { get; set; }
    public decimal Maximum { get; set; }
    public string FormatString { get; set; }
}