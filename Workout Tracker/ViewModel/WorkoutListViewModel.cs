using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Model;
using Workout_Tracker.Services;
using Workout_Tracker.View;

namespace Workout_Tracker.ViewModel;

public partial class WorkoutListViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private List<WorkoutDisplay> _allWorkouts = [];

    private readonly LoadingService _loading;

    public WorkoutListViewModel(DatabaseService db, LoadingService loading)
    {
        _db = db;
        _loading = loading;
    }

    public ObservableCollection<WorkoutDisplay> Workouts { get; } = [];

    [ObservableProperty]
    private string? _searchText;

    partial void OnSearchTextChanged(string? value) => ApplyFilter();

    public async Task LoadWorkoutsAsync()
    {
        await _loading.RunAsync(async () =>
        {
            _allWorkouts = await _db.GetAllWorkoutsAsync();
            ApplyFilter();
        }, "Loading...");
    }

    private void ApplyFilter()
    {
        var filtered = _allWorkouts.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
            filtered = filtered.Where(w =>
                w.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Workouts.Clear();
        foreach (var w in filtered)
            Workouts.Add(w);
    }

    [RelayCommand]
    private async Task GoToDetail(WorkoutDisplay workout)
    {
        await Shell.Current.GoToAsync($"{nameof(WorkoutDetailPage)}?id={workout.Id}");
    }

    [RelayCommand]
    private async Task GoToAdd()
    {
        await Shell.Current.GoToAsync(nameof(NewWorkoutPage));
    }
}
