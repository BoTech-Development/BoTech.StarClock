using System.Collections.Generic;

namespace BoTech.StarClock.ViewModels.Standard;

public class FormResult
{
    public Dictionary<string,object> Results { get; set; }
    public FormResultState ResultState { get; set; }
    public enum FormResultState
    {
        Accepted,
        Canceled,
        Rejected,
        Closed
    }
}