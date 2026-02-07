using CommunityToolkit.Mvvm.ComponentModel;

namespace Workout_Tracker.Model;

public partial class MuscleSelection : ObservableObject
{
    public Muscle Muscle { get; set; } = null!;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _role = "primary";
}
