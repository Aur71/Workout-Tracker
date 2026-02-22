using SQLite;

namespace Workout_Tracker.Model;

public class SessionExercise
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, Indexed]
    public int SessionId { get; set; }

    [NotNull, Indexed]
    public int ExerciseId { get; set; }

    [NotNull]
    public int Order { get; set; }

    public int? RestSeconds { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
