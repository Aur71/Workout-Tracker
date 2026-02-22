using Program = Workout_Tracker.Model.Program;

namespace Workout_Tracker.Model;

public class ProgramExportData
{
    public string Type { get; set; } = "program";
    public int Version { get; set; } = 1;
    public DateTime ExportDate { get; set; }
    public ProgramExportPayload Data { get; set; } = new();
}

public class ProgramExportPayload
{
    public Program Program { get; set; } = new();
    public List<Session> Sessions { get; set; } = [];
    public List<SessionExercise> SessionExercises { get; set; } = [];
    public List<Set> Sets { get; set; } = [];
    public List<Exercise> Exercises { get; set; } = [];
    public List<Muscle> Muscles { get; set; } = [];
    public List<ExerciseMuscle> ExerciseMuscles { get; set; } = [];
}
