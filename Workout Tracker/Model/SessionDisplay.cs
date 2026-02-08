namespace Workout_Tracker.Model;

public class SessionDisplay
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public int ExerciseCount { get; set; }
    public int SetCount { get; set; }
    public bool IsCompleted { get; set; }

    public string DateDisplay => Date.ToString("ddd, MMM d");

    public string ExerciseCountDisplay => ExerciseCount == 1 ? "1 exercise" : $"{ExerciseCount} exercises";

    public string SetCountDisplay => SetCount == 1 ? "1 set" : $"{SetCount} sets";

    public bool HasNotes => !string.IsNullOrWhiteSpace(Notes);

    public string NotesPreview => Notes?.Length > 60 ? Notes[..60] + "..." : Notes ?? "";

    public string StatusDisplay => IsCompleted ? "Completed" : "";

    public bool ShowCompletedBadge => IsCompleted;

    public Color CompletedBadgeBg => Application.Current!.RequestedTheme == AppTheme.Dark
        ? Color.FromArgb("#1A3D33") : Color.FromArgb("#E8FFF6");

    public Color CompletedBadgeTextColor => Color.FromArgb("#00D9A5");
}
