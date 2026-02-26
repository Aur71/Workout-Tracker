using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Workout_Tracker.Model;

public partial class MethodChipItem : ObservableObject
{
    public OverloadMethod Method { get; set; }
    public string Label { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isSelected;

    public SessionOverloadConfig? Owner { get; set; }

    [RelayCommand]
    private void Select()
    {
        Owner?.SelectMethod(Method);
    }
}
