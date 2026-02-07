namespace Workout_Tracker.Model;

public class WorkoutDisplay
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Goal { get; set; }
    public int? EstimatedDuration { get; set; }
    public string? Notes { get; set; }
    public List<WorkoutExerciseDisplay> Exercises { get; set; } = [];

    public string GoalDisplay => Goal switch
    {
        "strength" => "Strength",
        "hypertrophy" => "Hypertrophy",
        "power" => "Power",
        "endurance" => "Endurance",
        "recovery" => "Recovery",
        _ => Goal ?? ""
    };

    public bool HasGoal => !string.IsNullOrWhiteSpace(Goal);

    public Color GoalBadgeBg => Goal switch
    {
        "strength" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#1A3D33") : Color.FromArgb("#E8FFF6"),
        "hypertrophy" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#3D1A1A") : Color.FromArgb("#FFF0EE"),
        "power" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#2A2A1A") : Color.FromArgb("#FFF8EE"),
        "endurance" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#1A2A3D") : Color.FromArgb("#EEF4FF"),
        "recovery" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#2A1A3D") : Color.FromArgb("#F5EEFF"),
        _ => Colors.Transparent
    };

    public Color GoalBadgeTextColor => Goal switch
    {
        "strength" => Color.FromArgb("#00D9A5"),
        "hypertrophy" => Color.FromArgb("#FF6B5B"),
        "power" => Color.FromArgb("#F59E0B"),
        "endurance" => Color.FromArgb("#4A6CF7"),
        "recovery" => Color.FromArgb("#A855F7"),
        _ => Colors.Gray
    };

    public string DurationDisplay => EstimatedDuration.HasValue
        ? $"~{EstimatedDuration} min"
        : "";

    public string ExerciseCountDisplay
    {
        get
        {
            var count = Exercises.Count;
            return count == 1 ? "1 exercise" : $"{count} exercises";
        }
    }
}
