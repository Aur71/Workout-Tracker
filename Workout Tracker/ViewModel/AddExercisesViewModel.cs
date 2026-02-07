using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Workout_Tracker.Messages;
using Workout_Tracker.Model;
using Workout_Tracker.Services;

namespace Workout_Tracker.ViewModel;

public partial class AddExercisesViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private List<ExerciseSelection> _allExercises = [];

    public static List<ExerciseSelection>? PreviousSelections { get; set; }

    public AddExercisesViewModel(DatabaseService db)
    {
        _db = db;
    }

    public ObservableCollection<ExerciseSelection> Exercises { get; } = [];

    [ObservableProperty]
    private string? _searchText;

    [ObservableProperty]
    private int _selectedCount;

    public string ButtonText => SelectedCount > 0
        ? $"Add {SelectedCount} Exercise{(SelectedCount != 1 ? "s" : "")}"
        : "Add Exercises";

    partial void OnSearchTextChanged(string? value)
    {
        FilterExercises();
    }

    public async Task LoadExercisesAsync()
    {
        var exercises = await _db.GetAllExercisesAsync();
        var previous = PreviousSelections;

        _allExercises = exercises.Select(e =>
        {
            var prev = previous?.FirstOrDefault(p => p.Exercise.Id == e.Id);
            return new ExerciseSelection
            {
                Exercise = e,
                IsSelected = prev != null
            };
        }).ToList();

        PreviousSelections = null;
        FilterExercises();
        UpdateCount();
    }

    private void FilterExercises()
    {
        Exercises.Clear();
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allExercises
            : _allExercises.Where(e => e.Exercise.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var e in filtered)
            Exercises.Add(e);
    }

    [RelayCommand]
    private void ToggleExercise(ExerciseSelection exercise)
    {
        exercise.IsSelected = !exercise.IsSelected;
        UpdateCount();
    }

    [RelayCommand]
    private async Task Confirm()
    {
        var selected = _allExercises.Where(e => e.IsSelected).ToList();
        WeakReferenceMessenger.Default.Send(new ExerciseSelectionMessage(selected));
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void UpdateCount()
    {
        SelectedCount = _allExercises.Count(e => e.IsSelected);
        OnPropertyChanged(nameof(ButtonText));
    }
}
