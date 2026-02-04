using SQLite;

namespace Workout_Tracker.Model;

public class Tag
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
