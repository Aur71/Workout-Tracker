using SQLite;

namespace Workout_Tracker.Model;

public class Set
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, Indexed]
    public int SessionExerciseId { get; set; }

    [NotNull]
    public int SetNumber { get; set; }

    // Actual performed values
    public int? Reps { get; set; }

    public double? Weight { get; set; }

    public int? DurationSeconds { get; set; }

    public int? RestSeconds { get; set; }

    public double? Rpe { get; set; } // Rate of Perceived Exertion

    [NotNull]
    public bool Completed { get; set; }

    [NotNull]
    public bool IsWarmup { get; set; }

    // Planned values (rep range)
    public int? RepMin { get; set; }

    public int? RepMax { get; set; }

    public int? DurationMin { get; set; }

    public int? DurationMax { get; set; }

    public double? PlannedWeight { get; set; }

    public double? PlannedRpe { get; set; }
}
