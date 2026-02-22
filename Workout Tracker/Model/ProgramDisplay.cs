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

    public Color BarColor
    {
        get
        {
            if (Color is null) return Microsoft.Maui.Graphics.Color.FromArgb("#00D9A5");
            try { return Microsoft.Maui.Graphics.Color.FromArgb(Color); }
            catch { return Microsoft.Maui.Graphics.Color.FromArgb("#00D9A5"); }
        }
    }

    public string DateRangeDisplay
    {
        get
        {
            var start = StartDate.ToString("MMM d");
            if (EndDate.HasValue)
            {
                var end = EndDate.Value.Year == StartDate.Year
                    ? EndDate.Value.ToString("MMM d")
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

    public int SessionCount { get; set; }
    public int CompletedSessionCount { get; set; }

    public string SessionCountDisplay => SessionCount == 1 ? "1 session" : $"{SessionCount} sessions";
    public string CompletedCountDisplay => CompletedSessionCount == 1 ? "1 completed" : $"{CompletedSessionCount} completed";

    public bool HasDuration => EndDate.HasValue;

    public bool HasGoal => !string.IsNullOrWhiteSpace(Goal);

    public bool HasNotes => !string.IsNullOrWhiteSpace(Notes);

    public string StatusDisplay
    {
        get
        {
            var today = DateTime.Today;
            if (StartDate <= today && (!EndDate.HasValue || EndDate.Value >= today))
                return "Active";
            if (StartDate > today)
                return "Upcoming";
            return "Completed";
        }
    }

    public Color StatusBadgeBg
    {
        get
        {
            var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
            return StatusDisplay switch
            {
                "Active" => isDark ? Microsoft.Maui.Graphics.Color.FromArgb("#1A3D33") : Microsoft.Maui.Graphics.Color.FromArgb("#E8FFF6"),
                "Upcoming" => isDark ? Microsoft.Maui.Graphics.Color.FromArgb("#1A2A3D") : Microsoft.Maui.Graphics.Color.FromArgb("#EEF4FF"),
                _ => isDark ? Microsoft.Maui.Graphics.Color.FromArgb("#2A2A2A") : Microsoft.Maui.Graphics.Color.FromArgb("#F0F0F0")
            };
        }
    }

    public Color StatusBadgeTextColor => StatusDisplay switch
    {
        "Active" => Microsoft.Maui.Graphics.Color.FromArgb("#00D9A5"),
        "Upcoming" => Microsoft.Maui.Graphics.Color.FromArgb("#4A6CF7"),
        _ => Microsoft.Maui.Graphics.Color.FromArgb("#8A8A8A")
    };

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
