using Program = Workout_Tracker.Model.Program;

namespace Workout_Tracker.Model;

public class ExportData
{
    public int Version { get; set; } = 1;
    public DateTime ExportDate { get; set; }
    public ExportPayload Data { get; set; } = new();
}

public class ExportPayload
{
    public List<Tag> Tags { get; set; } = [];
    public List<Muscle> Muscles { get; set; } = [];
    public List<Exercise> Exercises { get; set; } = [];
    public List<ExerciseMuscle> ExerciseMuscles { get; set; } = [];
    public List<Program> Programs { get; set; } = [];
    public List<Session> Sessions { get; set; } = [];
    public List<SessionExercise> SessionExercises { get; set; } = [];
    public List<Set> Sets { get; set; } = [];
    public List<BodyMetric> BodyMetrics { get; set; } = [];
    public List<RecoveryLog> RecoveryLogs { get; set; } = [];
    public List<CalorieLog> CalorieLogs { get; set; } = [];
}
