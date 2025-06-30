using System.Collections.Generic;
using Avalonia.Controls;

namespace BoTech.StarClock.ViewModels.Standard.Form;

public class ComboBoxFormInput : FormInput
{
    private int selectedIndex = -1;
    public ComboBoxFormInput(string name, string helpText, List<ComboBoxItem> items) : base(name, helpText)
    {
        InputControl = new ComboBox();
        foreach (var item in items) ((ComboBox)InputControl).Items.Add(item);
        ((ComboBox)InputControl).SelectionChanged += (sender, args) => selectedIndex = ((ComboBox)InputControl).SelectedIndex;
    }
    public override object GetResult() => ((ComboBox)InputControl).Items[selectedIndex];
 
}