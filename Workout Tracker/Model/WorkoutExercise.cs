using SQLite;

namespace Workout_Tracker.Model;

public class WorkoutExercise
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, Indexed]
    public int WorkoutId { get; set; }

    [NotNull, Indexed]
    public int ExerciseId { get; set; }

    [NotNull]
    public int Order { get; set; }
}
