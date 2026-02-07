using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Workout_Tracker.Messages;
using Workout_Tracker.Model;
using Workout_Tracker.Services;

namespace Workout_Tracker.ViewModel;

public partial class SelectMusclesViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private List<MuscleSelection> _allMuscles = [];

    // Static property for passing previous selections from NewExerciseViewModel
    public static List<MuscleSelection>? PreviousSelections { get; set; }

    public SelectMusclesViewModel(DatabaseService db)
    {
        _db = db;
    }

    public ObservableCollection<MuscleSelection> Muscles { get; } = [];

    [ObservableProperty]
    private string? _searchText;

    [ObservableProperty]
    private int _selectedCount;

    public string ButtonText => SelectedCount > 0
        ? $"Add {SelectedCount} Muscle{(SelectedCount != 1 ? "s" : "")}"
        : "Add Muscles";

    partial void OnSearchTextChanged(string? value)
    {
        FilterMuscles();
    }

    public async Task LoadMusclesAsync()
    {
        var muscles = await _db.GetAllMusclesAsync();
        var previous = PreviousSelections;

        _allMuscles = muscles.Select(m =>
        {
            var prev = previous?.FirstOrDefault(p => p.Muscle.Id == m.Id);
            return new MuscleSelection
            {
                Muscle = m,
                IsSelected = prev != null,
                Role = prev?.Role ?? "primary"
            };
        }).ToList();

        PreviousSelections = null;
        FilterMuscles();
        UpdateCount();
    }

    private void FilterMuscles()
    {
        Muscles.Clear();
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allMuscles
            : _allMuscles.Where(m => m.Muscle.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var m in filtered)
            Muscles.Add(m);
    }

    [RelayCommand]
    private void ToggleMuscle(MuscleSelection muscle)
    {
        muscle.IsSelected = !muscle.IsSelected;
        if (muscle.IsSelected)
            muscle.Role = "primary";

        UpdateCount();
    }

    [RelayCommand]
    private void SetRole(MuscleSelection muscle)
    {
        if (!muscle.IsSelected) return;
        muscle.Role = muscle.Role == "primary" ? "secondary" : "primary";
    }

    [RelayCommand]
    private async Task Confirm()
    {
        var selected = _allMuscles.Where(m => m.IsSelected).ToList();
        WeakReferenceMessenger.Default.Send(new MuscleSelectionMessage(selected));
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void UpdateCount()
    {
        SelectedCount = _allMuscles.Count(m => m.IsSelected);
        OnPropertyChanged(nameof(ButtonText));
    }
}
