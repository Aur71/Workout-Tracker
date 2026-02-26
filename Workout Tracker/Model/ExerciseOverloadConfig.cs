using CommunityToolkit.Mvvm.ComponentModel;

namespace Workout_Tracker.Model;

public partial class ExerciseOverloadConfig : ObservableObject
{
    public int SessionIndex { get; set; }
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public string SessionLabel { get; set; } = string.Empty;
    public double? BaseWeight { get; set; }
    public int BaseSetCount { get; set; }
    public int? BaseRepMin { get; set; }
    public int? BaseRepMax { get; set; }
    public double? BaseRpe { get; set; }

    // Weight increment (Linear, Double, StepLoading)
    [ObservableProperty]
    private string _incrementText = "2.5";

    // Volume method
    [ObservableProperty]
    private string _setsToAddText = "1";

    [ObservableProperty]
    private string _everyNCyclesText = "1";

    public string BaseInfoDisplay
    {
        get
        {
            var parts = new List<string>();
            if (BaseSetCount > 0)
            {
                if (BaseRepMin.HasValue && BaseRepMax.HasValue && BaseRepMin != BaseRepMax)
                    parts.Add($"{BaseSetCount}x{BaseRepMin}-{BaseRepMax}");
                else if (BaseRepMin.HasValue)
                    parts.Add($"{BaseSetCount}x{BaseRepMin}");
                else
                    parts.Add($"{BaseSetCount} sets");
            }
            if (BaseWeight.HasValue)
                parts.Add($"{BaseWeight}kg");
            if (BaseRpe.HasValue)
                parts.Add($"RPE {BaseRpe}");
            return parts.Count > 0 ? string.Join(" | ", parts) : "No sets";
        }
    }
}
