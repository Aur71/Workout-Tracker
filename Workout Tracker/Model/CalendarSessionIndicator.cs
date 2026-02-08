namespace Workout_Tracker.Model;

public class CalendarSessionIndicator
{
    public int SessionId { get; set; }
    public int? ProgramId { get; set; }
    public string? ProgramName { get; set; }
    public string? ProgramColor { get; set; }
    public DateTime Date { get; set; }
    public int ExerciseCount { get; set; }
    public int SetCount { get; set; }

    public Color DotColor =>
        string.IsNullOrWhiteSpace(ProgramColor)
            ? Color.FromArgb("#00D9A5")
            : Color.FromArgb(ProgramColor);

    public string ProgramNameDisplay => ProgramName ?? "Unlinked Session";

    public string ExerciseCountDisplay =>
        ExerciseCount == 1 ? "1 exercise" : $"{ExerciseCount} exercises";

    public string SetCountDisplay =>
        SetCount == 1 ? "1 set" : $"{SetCount} sets";
}
