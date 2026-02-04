using SQLite;

namespace Workout_Tracker.Model;

public class Exercise
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(2000)]
    public string? Instructions { get; set; }

    [MaxLength(200)]
    public string? Equipment { get; set; }

    [MaxLength(50)]
    public string? ExerciseType { get; set; } // compound, isolation, cardio, mobility

    [MaxLength(500)]
    public string? ExampleMedia { get; set; }

    [NotNull]
    public bool IsTimeBased { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
