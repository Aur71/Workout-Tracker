using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Workout_Tracker.Model;

public partial class SessionOverloadConfig : ObservableObject
{
    public int SessionIndex { get; set; }
    public string SessionLabel { get; set; } = string.Empty;

    [ObservableProperty]
    private OverloadMethod _selectedMethod = OverloadMethod.Linear;

    [ObservableProperty]
    private string _rpeIncrementText = "0.5";

    [ObservableProperty]
    private string _stepCyclesText = "2";

    [ObservableProperty]
    private string _doubleProgressionCyclesText = "3";

    public ObservableCollection<ExerciseOverloadConfig> Exercises { get; } = [];
    public ObservableCollection<MethodChipItem> MethodChips { get; } = [];

    public bool ShowWeightIncrement => SelectedMethod is OverloadMethod.Linear
        or OverloadMethod.Double or OverloadMethod.StepLoading;

    public bool ShowVolumeConfig => SelectedMethod == OverloadMethod.Volume;
    public bool ShowRpeIncrement => SelectedMethod == OverloadMethod.Rpe;
    public bool ShowStepCycles => SelectedMethod == OverloadMethod.StepLoading;
    public bool ShowDoubleProgressionCycles => SelectedMethod == OverloadMethod.Double;

    partial void OnSelectedMethodChanged(OverloadMethod value)
    {
        OnPropertyChanged(nameof(ShowWeightIncrement));
        OnPropertyChanged(nameof(ShowVolumeConfig));
        OnPropertyChanged(nameof(ShowRpeIncrement));
        OnPropertyChanged(nameof(ShowStepCycles));
        OnPropertyChanged(nameof(ShowDoubleProgressionCycles));

        foreach (var chip in MethodChips)
            chip.IsSelected = chip.Method == value;
    }

    public void InitializeChips()
    {
        MethodChips.Clear();
        var methods = new (OverloadMethod method, string label)[]
        {
            (OverloadMethod.Linear, "Linear"),
            (OverloadMethod.Double, "Double"),
            (OverloadMethod.Volume, "Volume"),
            (OverloadMethod.Rpe, "RPE"),
            (OverloadMethod.StepLoading, "Step"),
        };

        foreach (var (method, label) in methods)
        {
            MethodChips.Add(new MethodChipItem
            {
                Method = method,
                Label = label,
                IsSelected = method == SelectedMethod,
                Owner = this
            });
        }
    }

    public void SelectMethod(OverloadMethod method)
    {
        SelectedMethod = method;
    }
}
