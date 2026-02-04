using SQLite;

namespace Workout_Tracker.Model;

public class CalorieLog
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, Indexed]
    public DateTime Date { get; set; }

    [MaxLength(50)]
    public string? ActivityLevel { get; set; }

    public int? TotalCalories { get; set; }
}
