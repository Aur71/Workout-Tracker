using SQLite;

namespace Workout_Tracker.Model;

public class SessionTag
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull, Indexed]
    public int SessionId { get; set; }

    [NotNull, MaxLength(100)]
    public string TagName { get; set; } = string.Empty;
}
