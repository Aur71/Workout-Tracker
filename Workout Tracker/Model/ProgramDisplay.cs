namespace Workout_Tracker.Model;

public class ProgramDisplay
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Goal { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
    public string? Color { get; set; }

    public Color BarColor => Color is not null
        ? Microsoft.Maui.Graphics.Color.FromArgb(Color)
        : Microsoft.Maui.Graphics.Color.FromArgb("#00D9A5");

    public string DateRangeDisplay
    {
        get
        {
            var start = StartDate.ToString("MMM d");
            if (EndDate.HasValue)
            {
                var end = EndDate.Value.Year == StartDate.Year
                    ? EndDate.Value.ToString("MMM d, yyyy")
                    : EndDate.Value.ToString("MMM d, yyyy");
                return $"{start} - {end}";
            }
            return $"{start}, {StartDate.Year} - Open";
        }
    }

    public string DurationDisplay
    {
        get
        {
            if (!EndDate.HasValue) return "";
            var days = (EndDate.Value - StartDate).Days;
            if (days < 7) return $"{days} days";
            var weeks = days / 7;
            return weeks == 1 ? "1 week" : $"{weeks} weeks";
        }
    }

    public bool HasDuration => EndDate.HasValue;

    public bool HasGoal => !string.IsNullOrWhiteSpace(Goal);

    public double ProgressFraction
    {
        get
        {
            if (!EndDate.HasValue) return 0;
            var total = (EndDate.Value - StartDate).TotalDays;
            if (total <= 0) return 1;
            var elapsed = (DateTime.Today - StartDate).TotalDays;
            return Math.Clamp(elapsed / total, 0, 1);
        }
    }

    public string ProgressPercentDisplay => $"{(int)(ProgressFraction * 100)}%";

    public string WeekProgressDisplay
    {
        get
        {
            if (!EndDate.HasValue) return "";
            var totalDays = (EndDate.Value - StartDate).Days;
            var elapsedDays = (DateTime.Today - StartDate).Days;
            var totalWeeks = Math.Max(1, (int)Math.Ceiling(totalDays / 7.0));
            var currentWeek = Math.Clamp((int)Math.Ceiling((elapsedDays + 1) / 7.0), 1, totalWeeks);
            return $"Week {currentWeek} of {totalWeeks}";
        }
    }
}
