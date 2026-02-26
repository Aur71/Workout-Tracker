namespace Workout_Tracker.Model;

public class OverloadPreviewCycle
{
    public int CycleNumber { get; set; }
    public int SessionCount { get; set; }
    public string WeekLabel { get; set; } = "";
    public List<OverloadPreviewExercise> Exercises { get; set; } = [];
}
