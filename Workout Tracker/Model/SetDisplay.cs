using CommunityToolkit.Mvvm.ComponentModel;

namespace Workout_Tracker.Model;

public partial class SetDisplay : ObservableObject
{
    [ObservableProperty]
    private int _setNumber;

    [ObservableProperty]
    private string _repMinText = "";

    [ObservableProperty]
    private string _repMaxText = "";

    [ObservableProperty]
    private string _durationMinText = "";

    [ObservableProperty]
    private string _durationMaxText = "";

    [ObservableProperty]
    private bool _isWarmup;

    public string DisplayLabel => IsWarmup ? "W" : SetNumber.ToString();

    partial void OnSetNumberChanged(int value) => OnPropertyChanged(nameof(DisplayLabel));
    partial void OnIsWarmupChanged(bool value) => OnPropertyChanged(nameof(DisplayLabel));
}
