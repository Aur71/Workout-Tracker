using SQLite;

namespace Workout_Tracker.Model;

public class Muscle
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, MaxLength(100), Unique]
    public string Name { get; set; } = string.Empty;
}
