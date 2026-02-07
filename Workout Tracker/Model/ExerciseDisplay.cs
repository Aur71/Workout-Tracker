namespace Workout_Tracker.Model;

public class ExerciseDisplay
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ExerciseType { get; set; }
    public string? Equipment { get; set; }
    public bool IsTimeBased { get; set; }
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public string? Notes { get; set; }
    public string? PrimaryMuscle { get; set; }
    public List<MuscleDisplay> Muscles { get; set; } = [];

    public string TypeDisplay => ExerciseType switch
    {
        "compound" => "Compound",
        "isolation" => "Isolation",
        "cardio" => "Cardio",
        "mobility" => "Mobility",
        _ => ExerciseType ?? ""
    };

    public Color TypeBadgeBg => ExerciseType switch
    {
        "compound" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#1A3D33") : Color.FromArgb("#E8FFF6"),
        "isolation" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#3D1A1A") : Color.FromArgb("#FFF0EE"),
        "cardio" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#1A2A3D") : Color.FromArgb("#EEF4FF"),
        "mobility" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#2A2A1A") : Color.FromArgb("#FFF8EE"),
        _ => Colors.Transparent
    };

    public Color TypeBadgeTextColor => ExerciseType switch
    {
        "compound" => Color.FromArgb("#00D9A5"),
        "isolation" => Color.FromArgb("#FF6B5B"),
        "cardio" => Color.FromArgb("#4A6CF7"),
        "mobility" => Color.FromArgb("#F59E0B"),
        _ => Colors.Gray
    };

    public string EquipmentDisplay => Equipment ?? "No Equipment";
}
