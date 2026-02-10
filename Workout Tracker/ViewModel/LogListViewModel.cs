using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Model;
using Workout_Tracker.Services;
using Workout_Tracker.View;

namespace Workout_Tracker.ViewModel;

public partial class LogListViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    private readonly LoadingService _loading;

    public LogListViewModel(DatabaseService db, LoadingService loading)
    {
        _db = db;
        _loading = loading;
    }

    public ObservableCollection<LogGroup> LogGroups { get; } = [];

    [ObservableProperty]
    private bool _isEmpty;

    public async Task LoadLogsAsync()
    {
        await _loading.RunAsync(async () =>
        {
            var entries = await _db.GetAllLogsAsync();

            LogGroups.Clear();

            var groups = entries
                .GroupBy(e => e.Date.Date)
                .OrderByDescending(g => g.Key)
                .Select(g => new LogGroup
                {
                    DateHeader = g.Key.ToString("MMMM d, yyyy"),
                    Entries = g.ToList()
                });

            foreach (var group in groups)
                LogGroups.Add(group);

            IsEmpty = LogGroups.Count == 0;
        }, "Loading...");
    }

    [RelayCommand]
    private async Task GoToAdd()
    {
        var page = Shell.Current.CurrentPage;
        var result = await page.DisplayActionSheet("New Log", "Cancel", null,
            "Body Metrics", "Recovery", "Calories");

        if (string.IsNullOrEmpty(result) || result == "Cancel")
            return;

        var type = result switch
        {
            "Body Metrics" => "body_metric",
            "Recovery" => "recovery",
            "Calories" => "calorie",
            _ => ""
        };

        if (!string.IsNullOrEmpty(type))
            await Shell.Current.GoToAsync($"{nameof(NewLogPage)}?type={type}");
    }

    [RelayCommand]
    private async Task GoToEdit(LogEntry entry)
    {
        await Shell.Current.GoToAsync($"{nameof(NewLogPage)}?type={entry.LogType}&id={entry.Id}");
    }

    [RelayCommand]
    private async Task DeleteLog(LogEntry entry)
    {
        var page = Shell.Current.CurrentPage;
        bool confirm = await page.DisplayAlert("Delete Log",
            "Are you sure you want to delete this log entry?", "Delete", "Cancel");

        if (!confirm) return;

        await _loading.RunAsync(async () =>
        {
            switch (entry.LogType)
            {
                case "body_metric":
                    await _db.DeleteBodyMetricAsync(entry.Id);
                    break;
                case "recovery":
                    await _db.DeleteRecoveryLogAsync(entry.Id);
                    break;
                case "calorie":
                    await _db.DeleteCalorieLogAsync(entry.Id);
                    break;
            }

            await LoadLogsAsync();
        }, "Deleting...");
    }
}
