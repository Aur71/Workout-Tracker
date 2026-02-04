using SQLite;

namespace Workout_Tracker.Model;

public class ExerciseMuscle
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, Indexed]
    public int ExerciseId { get; set; }

    [NotNull, Indexed]
    public int MuscleId { get; set; }

    [MaxLength(50)]
    public string? Type { get; set; } // primary, secondary, etc.
}
