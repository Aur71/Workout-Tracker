using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Model;
using Workout_Tracker.Services;

namespace Workout_Tracker.ViewModel;

public partial class NewLogViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private int? _editId;

    private readonly LoadingService _loading;

    public NewLogViewModel(DatabaseService db, LoadingService loading)
    {
        _db = db;
        _loading = loading;
    }

    [ObservableProperty]
    private string _logType = "";

    [ObservableProperty]
    private DateTime _date = DateTime.Today;

    // Body Metric fields
    [ObservableProperty]
    private string _bodyweightText = "";

    [ObservableProperty]
    private string _bodyFatText = "";

    [ObservableProperty]
    private string _notes = "";

    // Recovery fields
    [ObservableProperty]
    private string _sleepHoursText = "";

    [ObservableProperty]
    private int? _sorenessLevel;

    [ObservableProperty]
    private int? _stressLevel;

    // Calorie fields
    [ObservableProperty]
    private string _totalCaloriesText = "";

    [ObservableProperty]
    private string? _selectedActivityLevel;

    public bool IsBodyMetric => LogType == "body_metric";
    public bool IsRecovery => LogType == "recovery";
    public bool IsCalorie => LogType == "calorie";

    public bool IsEditMode => _editId.HasValue;

    public List<string> ActivityLevelOptions { get; } =
        ["Sedentary", "Light", "Moderate", "Active", "Very Active"];

    public List<int> LevelOptions { get; } = [1, 2, 3, 4, 5];

    partial void OnLogTypeChanged(string value)
    {
        OnPropertyChanged(nameof(IsBodyMetric));
        OnPropertyChanged(nameof(IsRecovery));
        OnPropertyChanged(nameof(IsCalorie));
    }

    public void SetType(string type)
    {
        LogType = type;
    }

    public async Task LoadLogAsync(string type, int id)
    {
        _editId = id;
        LogType = type;

        switch (type)
        {
            case "body_metric":
                var metric = await _db.GetBodyMetricByIdAsync(id);
                if (metric != null)
                {
                    Date = metric.Date;
                    BodyweightText = metric.Bodyweight?.ToString() ?? "";
                    BodyFatText = metric.BodyFat?.ToString() ?? "";
                    Notes = metric.Notes ?? "";
                }
                break;

            case "recovery":
                var recovery = await _db.GetRecoveryLogByIdAsync(id);
                if (recovery != null)
                {
                    Date = recovery.Date;
                    SleepHoursText = recovery.SleepHours?.ToString() ?? "";
                    SorenessLevel = recovery.SorenessLevel;
                    StressLevel = recovery.StressLevel;
                }
                break;

            case "calorie":
                var calorie = await _db.GetCalorieLogByIdAsync(id);
                if (calorie != null)
                {
                    Date = calorie.Date;
                    TotalCaloriesText = calorie.TotalCalories?.ToString() ?? "";
                    SelectedActivityLevel = calorie.ActivityLevel;
                }
                break;
        }

        OnPropertyChanged(nameof(IsEditMode));
    }

    [RelayCommand]
    private void SelectSoreness(int level)
    {
        SorenessLevel = level;
    }

    [RelayCommand]
    private void SelectStress(int level)
    {
        StressLevel = level;
    }

    [RelayCommand]
    private void SelectActivityLevel(string level)
    {
        SelectedActivityLevel = level;
    }

    [RelayCommand]
    private async Task Save()
    {
        bool hasData = LogType switch
        {
            "body_metric" => !string.IsNullOrWhiteSpace(BodyweightText) || !string.IsNullOrWhiteSpace(BodyFatText) || !string.IsNullOrWhiteSpace(Notes),
            "recovery" => !string.IsNullOrWhiteSpace(SleepHoursText) || SorenessLevel.HasValue || StressLevel.HasValue,
            "calorie" => !string.IsNullOrWhiteSpace(TotalCaloriesText) || SelectedActivityLevel != null,
            _ => false
        };

        if (!hasData)
        {
            await Shell.Current.DisplayAlertAsync("Validation", "Please enter at least one value.", "OK");
            return;
        }

        await _loading.RunAsync(async () =>
        {
        switch (LogType)
        {
            case "body_metric":
                double.TryParse(BodyweightText, out var weight);
                double.TryParse(BodyFatText, out var bf);
                var metric = new BodyMetric
                {
                    Date = Date,
                    Bodyweight = weight > 0 ? weight : null,
                    BodyFat = bf > 0 ? bf : null,
                    Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim()
                };
                if (_editId.HasValue)
                {
                    metric.Id = _editId.Value;
                    await _db.UpdateBodyMetricAsync(metric);
                }
                else
                {
                    await _db.SaveBodyMetricAsync(metric);
                }
                break;

            case "recovery":
                double.TryParse(SleepHoursText, out var sleep);
                var recovery = new RecoveryLog
                {
                    Date = Date,
                    SleepHours = sleep > 0 ? sleep : null,
                    SorenessLevel = SorenessLevel,
                    StressLevel = StressLevel
                };
                if (_editId.HasValue)
                {
                    recovery.Id = _editId.Value;
                    await _db.UpdateRecoveryLogAsync(recovery);
                }
                else
                {
                    await _db.SaveRecoveryLogAsync(recovery);
                }
                break;

            case "calorie":
                int.TryParse(TotalCaloriesText, out var cal);
                var calorie = new CalorieLog
                {
                    Date = Date,
                    TotalCalories = cal > 0 ? cal : null,
                    ActivityLevel = SelectedActivityLevel
                };
                if (_editId.HasValue)
                {
                    calorie.Id = _editId.Value;
                    await _db.UpdateCalorieLogAsync(calorie);
                }
                else
                {
                    await _db.SaveCalorieLogAsync(calorie);
                }
                break;
        }

        await Shell.Current.GoToAsync("..");
        }, "Saving...");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
