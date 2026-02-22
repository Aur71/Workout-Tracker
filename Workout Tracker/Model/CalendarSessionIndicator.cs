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

    public string ProgramNameDisplay => ProgramName ?? "Unlinked Session";

    public string ExerciseCountDisplay =>
        ExerciseCount == 1 ? "1 exercise" : $"{ExerciseCount} exercises";

    public string SetCountDisplay =>
        SetCount == 1 ? "1 set" : $"{SetCount} sets";
}
