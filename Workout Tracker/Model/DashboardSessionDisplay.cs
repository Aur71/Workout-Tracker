namespace Workout_Tracker.Model;

public class DashboardSessionDisplay
{
    public int SessionId { get; set; }
    public int? ProgramId { get; set; }
    public string? ProgramName { get; set; }
    public string? ProgramColor { get; set; }
    public DateTime Date { get; set; }
    public int ExerciseCount { get; set; }
    public int SetCount { get; set; }
    public bool IsCompleted { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }

    public string StatusDisplay => IsCompleted ? "Completed" : "Scheduled";

    public Color DotColor
    {
        get
        {
            if (string.IsNullOrWhiteSpace(ProgramColor))
                return Color.FromArgb("#00D9A5");
            try { return Color.FromArgb(ProgramColor); }
            catch { return Color.FromArgb("#00D9A5"); }
        }
    }

    public string DateDisplay => Date.ToString("ddd, MMM d");

    public string ExerciseCountDisplay =>
        ExerciseCount == 1 ? "1 exercise" : $"{ExerciseCount} exercises";

    public string SetCountDisplay =>
        SetCount == 1 ? "1 set" : $"{SetCount} sets";

    public string ProgramNameDisplay => ProgramName ?? "Unlinked Session";

    public Color StatusBadgeBg => IsCompleted
        ? (Application.Current?.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#1A3D33") : Color.FromArgb("#E8FFF6"))
        : (Application.Current?.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#1A2A3D") : Color.FromArgb("#EEF4FF"));

    public Color StatusBadgeTextColor => IsCompleted
        ? Color.FromArgb("#00D9A5")
        : Color.FromArgb("#4A6CF7");

    public string DayOfWeekShort => Date.ToString("ddd").ToUpper();
    public string DayNumber => Date.Day.ToString();

    public bool IsStartable => !IsCompleted && Date.Date == DateTime.Today;
    public bool ShowCompletedBadge => IsCompleted;
}
