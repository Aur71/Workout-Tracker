using SQLite;

namespace Workout_Tracker.Model;

public class RecoveryLog
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, Indexed]
    public DateTime Date { get; set; }

    public double? SleepHours { get; set; }

    public int? SorenessLevel { get; set; } // 1-5

    public int? StressLevel { get; set; } // 1-5
}
