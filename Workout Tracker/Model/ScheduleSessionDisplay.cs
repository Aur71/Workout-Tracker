using CommunityToolkit.Mvvm.ComponentModel;

namespace Workout_Tracker.Model;

public partial class ScheduleSessionDisplay : ObservableObject
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int ExerciseCount { get; set; }
    public int SetCount { get; set; }
    public bool IsCompleted { get; set; }
    public List<string> Tags { get; set; } = [];

    [ObservableProperty]
    private DateTime _editDate;

    public string DateDisplay => Date.ToString("ddd, MMM d");

    public string ExerciseCountDisplay => ExerciseCount == 1 ? "1 exercise" : $"{ExerciseCount} exercises";

    public string SetCountDisplay => SetCount == 1 ? "1 set" : $"{SetCount} sets";

    public bool HasTags => Tags.Count > 0;
    public string TagsDisplay => string.Join(" \u00B7 ", Tags);

    public bool ShowCompletedBadge => IsCompleted;

    public Color CompletedBadgeBg => Application.Current!.RequestedTheme == AppTheme.Dark
        ? Color.FromArgb("#1A3D33") : Color.FromArgb("#E8FFF6");

    public Color CompletedBadgeTextColor => Color.FromArgb("#00D9A5");

    public bool HasChanged => EditDate.Date != Date.Date;
}
