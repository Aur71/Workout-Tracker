namespace Workout_Tracker.Model;

public class LogEntry
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string LogType { get; set; } = "";
    public string Summary { get; set; } = "";

    public string DateDisplay => Date.ToString("h:mm tt").ToLower();

    public string TypeDisplay => LogType switch
    {
        "body_metric" => "Body",
        "recovery" => "Recovery",
        "calorie" => "Calories",
        _ => "Log"
    };

    public Color TypeBadgeBg => LogType switch
    {
        "body_metric" => Color.FromArgb("#4A6CF7"),
        "recovery" => Color.FromArgb("#00D9A5"),
        "calorie" => Color.FromArgb("#F59E0B"),
        _ => Colors.Gray
    };

    public Color TypeBadgeTextColor => LogType switch
    {
        "body_metric" => Colors.White,
        "recovery" => Colors.Black,
        "calorie" => Colors.Black,
        _ => Colors.White
    };
}

public class LogGroup
{
    public string DateHeader { get; set; } = "";
    public List<LogEntry> Entries { get; set; } = [];
}
