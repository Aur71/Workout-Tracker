using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Workout_Tracker.Helpers;

namespace Workout_Tracker.Model;

public partial class ActiveExerciseDisplay : ObservableObject
{
    public int SessionExerciseId { get; set; }
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public string? ExerciseType { get; set; }
    public string? PrimaryMuscle { get; set; }
    public bool IsTimeBased { get; set; }
    public int Order { get; set; }
    public string? Notes { get; set; }
    public string? ExampleMedia { get; set; }
    public int RestSeconds { get; set; } = 120;

    public string RestDisplay
    {
        get
        {
            var mins = RestSeconds / 60;
            var secs = RestSeconds % 60;
            return secs > 0 ? $"Rest: {mins}:{secs:D2}" : $"Rest: {mins}:00";
        }
    }

    [ObservableProperty]
    private bool _isExpanded = true;

    public bool IsReadOnly { get; set; }
    public bool IsEditable => !IsReadOnly;

    public ObservableCollection<ActiveSetDisplay> Sets { get; } = [];

    public ActiveExerciseDisplay()
    {
        Sets.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(CompletedSetCountDisplay));
            OnPropertyChanged(nameof(TotalSetCount));
            OnPropertyChanged(nameof(CompletedSetCount));
        };
    }

    public int TotalSetCount => Sets.Count;
    public int CompletedSetCount => Sets.Count(s => s.Completed);

    public string CompletedSetCountDisplay => $"{CompletedSetCount}/{TotalSetCount}";

    public string TypeDisplay => ExerciseType switch
    {
        "compound" => "Compound",
        "isolation" => "Isolation",
        "cardio" => "Cardio",
        "mobility" => "Mobility",
        "plyometric" => "Plyometric",
        _ => ExerciseType ?? ""
    };

    public Color TypeBadgeBg => ExerciseType switch
    {
        "compound" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#1A3D33") : Color.FromArgb("#E8FFF6"),
        "isolation" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#3D1A1A") : Color.FromArgb("#FFF0EE"),
        "cardio" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#1A2A3D") : Color.FromArgb("#EEF4FF"),
        "mobility" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#2A2A1A") : Color.FromArgb("#FFF8EE"),
        "plyometric" => Application.Current!.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#2D1A3D") : Color.FromArgb("#F3EEFF"),
        _ => Colors.Transparent
    };

    public Color TypeBadgeTextColor => ExerciseType switch
    {
        "compound" => Color.FromArgb("#00D9A5"),
        "isolation" => Color.FromArgb("#FF6B5B"),
        "cardio" => Color.FromArgb("#4A6CF7"),
        "mobility" => Color.FromArgb("#F59E0B"),
        "plyometric" => Color.FromArgb("#8B5CF6"),
        _ => Colors.Gray
    };

    public bool HasVideo => YouTubeHelper.IsValidYouTubeUrl(ExampleMedia);
    public string? VideoEmbedUrl => YouTubeHelper.GetEmbedUrl(ExampleMedia);

    public void RefreshCompletedCount()
    {
        OnPropertyChanged(nameof(CompletedSetCountDisplay));
        OnPropertyChanged(nameof(CompletedSetCount));
    }
}
