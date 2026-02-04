using SQLite;

namespace Workout_Tracker.Model;

public class BodyMetric
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, Indexed]
    public DateTime Date { get; set; }

    public double? Bodyweight { get; set; }

    public double? BodyFat { get; set; } // percentage

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
