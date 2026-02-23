using CommunityToolkit.Mvvm.ComponentModel;

namespace Workout_Tracker.Model;

public partial class ActiveSetDisplay : ObservableObject
{
    public int SetId { get; set; }
    public int SetNumber { get; set; }
    public bool IsWarmup { get; set; }

    // Read-only planned targets
    public int? PlannedRepMin { get; set; }
    public int? PlannedRepMax { get; set; }
    public int? PlannedDurationMin { get; set; }
    public int? PlannedDurationMax { get; set; }

    public string PlannedRepDisplay =>
        PlannedRepMin.HasValue && PlannedRepMax.HasValue
            ? $"{PlannedRepMin}-{PlannedRepMax}"
            : PlannedRepMin?.ToString() ?? PlannedRepMax?.ToString() ?? "-";

    public string PlannedDurationDisplay =>
        PlannedDurationMin.HasValue && PlannedDurationMax.HasValue
            ? $"{PlannedDurationMin}-{PlannedDurationMax}s"
            : PlannedDurationMin.HasValue ? $"{PlannedDurationMin}s"
            : PlannedDurationMax.HasValue ? $"{PlannedDurationMax}s"
            : "-";

    public string DisplayLabel => IsWarmup ? "W" : SetNumber.ToString();

    // User input fields
    [ObservableProperty]
    private string _repsText = "";

    [ObservableProperty]
    private string _weightText = "";

    [ObservableProperty]
    private string _durationText = "";

    [ObservableProperty]
    private string _rpeText = "";

    public bool Completed =>
        (int.TryParse(RepsText, out var r) && r > 0) ||
        (int.TryParse(DurationText, out var d) && d > 0);

    partial void OnRepsTextChanged(string value)
    {
        OnPropertyChanged(nameof(Completed));
    }

    partial void OnDurationTextChanged(string value)
    {
        OnPropertyChanged(nameof(Completed));
    }
}
