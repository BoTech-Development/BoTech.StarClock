using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using BoTech.StarClock.ViewModels.Standard.Form;
using ReactiveUI;

namespace BoTech.StarClock.ViewModels.Standard;

public class FormBuilderViewModel : ViewModelBase
{
    public string FormName { get; set; }
    public List<FormInput> FormInputs { get; set; }
    public ReactiveCommand<Unit, Unit> AcceptCommand { get; set; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; set; } 
    /// <summary>
    /// Will be true when the user clicks the ok Button
    /// </summary>
    private bool _accepted = false;
    /// <summary>
    /// Will be true when the user clicks the cancel Button
    /// </summary>
    private bool _canceled = false;
    /// <summary>
    /// the results of the Form after submitting the form.
    /// </summary>
    private Dictionary<string, object>? _results = null;
    public FormBuilderViewModel(string formName, List<FormInput> formInputs)
    {
        FormName = formName;
        List<string> inputNames = new List<string>();
        foreach (var formInput in formInputs) if(inputNames.Contains(formInput.Name)) throw new ArgumentException("All FormInput Names must be unique. Name:" + formInput.Name +" already exists"); else inputNames.Add(formInput.Name);
        FormInputs = formInputs;
        
        AcceptCommand = ReactiveCommand.Create(Accept);
        CancelCommand = ReactiveCommand.Create(() =>
        {
             _canceled = true;
        });
    }
    

    /// <summary>
    /// Reserved for Development
    /// </summary>
    public FormBuilderViewModel() : this("Test",  new List<FormInput>())
    {
        FormInputs.Add(new TextFormInput("Name", "The name of the timer.", "Enter the name of the Timer."));
        
    }

    public Task<FormResult> Show()
    {
        return Task.Run(() =>
        {
            while (!_accepted || !_canceled)
            {

            }

            if (_results != null)
            {
                return new FormResult()
                {
                    Results = _results,
                    ResultState = FormResult.FormResultState.Accepted,
                };
            }

            return new FormResult()
            {
                ResultState = FormResult.FormResultState.Canceled,
                Results = null
            };
        });
    }
    /// <summary>
    /// Will be called when the user clicks the ok button
    /// This method enumerates all results
    /// </summary>
    private void Accept()
    {
        Dictionary<string, object> results = new Dictionary<string, object>();
        foreach (FormInput formInput in FormInputs) results.Add(formInput.Name, formInput.GetResult());
        _results = results;
        _accepted = true;
    }
}
