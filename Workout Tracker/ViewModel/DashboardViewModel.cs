using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Model;
using Workout_Tracker.Services;
using Workout_Tracker.View;

namespace Workout_Tracker.ViewModel;

public partial class DashboardViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    private readonly LoadingService _loading;

    public DashboardViewModel(DatabaseService db, LoadingService loading)
    {
        _db = db;
        _loading = loading;
    }

    public ObservableCollection<DashboardSessionDisplay> TodaySessions { get; } = [];
    public ObservableCollection<DashboardSessionDisplay> UpcomingSessions { get; } = [];

    [ObservableProperty]
    private string _greetingText = "Good morning,";

    [ObservableProperty]
    private string _todayDateDisplay = string.Empty;

    [ObservableProperty]
    private bool _hasTodaySessions;

    [ObservableProperty]
    private bool _hasUpcomingSessions;

    [ObservableProperty]
    private string _weeklyWorkoutCount = "0";

    [ObservableProperty]
    private string _weeklySetCount = "0";

    [ObservableProperty]
    private string _weeklyVolumeDisplay = "0";

    public async Task LoadAsync()
    {
        await _loading.RunAsync(async () =>
        {
        // Time-based greeting
        var hour = DateTime.Now.Hour;
        GreetingText = hour switch
        {
            < 12 => "Good morning",
            < 17 => "Good afternoon",
            _ => "Good evening"
        };

        TodayDateDisplay = DateTime.Today.ToString("dddd, MMM d");

        // Load today's sessions
        var todaySessions = await _db.GetSessionsForDateAsync(DateTime.Today);
        TodaySessions.Clear();
        foreach (var s in todaySessions)
            TodaySessions.Add(s);
        HasTodaySessions = TodaySessions.Count > 0;

        // Load upcoming sessions
        var upcoming = await _db.GetUpcomingSessionsAsync(7);
        UpcomingSessions.Clear();
        foreach (var s in upcoming)
            UpcomingSessions.Add(s);
        HasUpcomingSessions = UpcomingSessions.Count > 0;

        // Load weekly stats
        var (workouts, sets, volume) = await _db.GetWeeklyStatsAsync();
        WeeklyWorkoutCount = workouts.ToString();
        WeeklySetCount = sets.ToString();
        WeeklyVolumeDisplay = volume >= 1000 ? $"{volume / 1000.0:0.#}k" : volume.ToString("0");
        }, "Loading...");
    }

    [RelayCommand]
    private async Task StartWorkout(DashboardSessionDisplay session)
    {
        await Shell.Current.GoToAsync($"{nameof(ActiveWorkoutPage)}?sessionId={session.SessionId}");
    }

    [RelayCommand]
    private async Task ViewSession(DashboardSessionDisplay session)
    {
        await Shell.Current.GoToAsync($"{nameof(ActiveWorkoutPage)}?sessionId={session.SessionId}");
    }
}
