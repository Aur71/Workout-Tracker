using SQLite;

namespace Workout_Tracker.Model;

public class Program
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Goal { get; set; }

    [NotNull]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [MaxLength(20)]
    public string? Color { get; set; }
}
