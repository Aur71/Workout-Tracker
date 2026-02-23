namespace Workout_Tracker.Model;

public class MuscleVolumeData
{
    public string MuscleName { get; set; } = string.Empty;
    public double TotalSets { get; set; }
    public double TotalVolume { get; set; }
}

public class ExerciseProgressionPoint
{
    public DateTime Date { get; set; }
    public double BestWeight { get; set; }
    public double EstimatedOneRepMax { get; set; }
    public double TotalVolume { get; set; }
    public int TotalSets { get; set; }
    public int TotalReps { get; set; }
    public string? ProgramGoal { get; set; }
}

public class SessionAdherenceData
{
    public DateTime Date { get; set; }
    public int PlannedSets { get; set; }
    public int CompletedSets { get; set; }
    public bool SessionCompleted { get; set; }
}

public class AnalyticsSummary
{
    public int TotalSessions { get; set; }
    public int TotalExercises { get; set; }
    public int TotalSetsCompleted { get; set; }
    public double TotalVolume { get; set; }
    public double AvgSessionDurationMinutes { get; set; }
    public double AvgEnergyLevel { get; set; }
}

public class BodyMetricPoint
{
    public DateTime Date { get; set; }
    public double? Bodyweight { get; set; }
    public double? BodyFat { get; set; }
}

public class BodyCompositionPoint
{
    public DateTime Date { get; set; }
    public double? Bodyweight { get; set; }
    public double? BodyFat { get; set; }
    public double? Calories { get; set; }
}

public class ExercisePickerItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public override string ToString() => Name;
}
