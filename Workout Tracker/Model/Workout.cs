using SQLite;

namespace Workout_Tracker.Model;

public class Workout
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Goal { get; set; } // strength, hypertrophy, endurance, recovery (deload)

    public int? EstimatedDuration { get; set; } // in minutes

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
