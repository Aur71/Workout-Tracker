using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Model;
using Workout_Tracker.Services;
using Workout_Tracker.View;

namespace Workout_Tracker.ViewModel;

public partial class ExerciseListViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private List<ExerciseDisplay> _allExercises = [];

    public ExerciseListViewModel(DatabaseService db)
    {
        _db = db;
    }

    public ObservableCollection<ExerciseDisplay> Exercises { get; } = [];

    [ObservableProperty]
    private string? _searchText;

    [ObservableProperty]
    private string _selectedFilter = "All";

    partial void OnSearchTextChanged(string? value) => ApplyFilters();

    public async Task LoadExercisesAsync()
    {
        _allExercises = await _db.GetAllExercisesAsync();
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = _allExercises.AsEnumerable();

        if (SelectedFilter != "All")
            filtered = filtered.Where(e =>
                string.Equals(e.ExerciseType, SelectedFilter, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(SearchText))
            filtered = filtered.Where(e =>
                e.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Exercises.Clear();
        foreach (var e in filtered)
            Exercises.Add(e);
    }

    [RelayCommand]
    private void Filter(string type)
    {
        SelectedFilter = type;
        ApplyFilters();
    }

    [RelayCommand]
    private async Task GoToDetail(ExerciseDisplay exercise)
    {
        await Shell.Current.GoToAsync($"{nameof(ExerciseDetailPage)}?id={exercise.Id}");
    }

    [RelayCommand]
    private async Task GoToAdd()
    {
        await Shell.Current.GoToAsync(nameof(NewExercisePage));
    }
}
