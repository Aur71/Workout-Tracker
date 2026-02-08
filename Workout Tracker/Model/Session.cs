using SQLite;

namespace Workout_Tracker.Model;

public class Session
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int? ProgramId { get; set; }

    [NotNull, Indexed]
    public DateTime Date { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public int? EnergyLevel { get; set; } // 1-5

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public int? Week { get; set; }

    public int? Day { get; set; }

    public bool IsCompleted { get; set; }
}
