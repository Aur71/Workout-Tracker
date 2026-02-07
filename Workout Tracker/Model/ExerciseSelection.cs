using CommunityToolkit.Mvvm.ComponentModel;

namespace Workout_Tracker.Model;

public partial class ExerciseSelection : ObservableObject
{
    public ExerciseDisplay Exercise { get; set; } = null!;

    [ObservableProperty]
    private bool _isSelected;
}
