namespace Workout_Tracker.Model;

public class MuscleDisplay
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = "primary";

    public bool IsPrimary => Role == "primary";
}
