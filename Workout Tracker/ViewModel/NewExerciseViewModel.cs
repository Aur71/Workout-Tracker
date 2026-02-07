using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Workout_Tracker.Messages;
using Workout_Tracker.Model;
using Workout_Tracker.Services;
using Workout_Tracker.View;

namespace Workout_Tracker.ViewModel;

public partial class NewExerciseViewModel : ObservableObject, IRecipient<MuscleSelectionMessage>
{
    private readonly DatabaseService _db;

    public NewExerciseViewModel(DatabaseService db)
    {
        _db = db;
        WeakReferenceMessenger.Default.Register(this);
    }

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _exerciseType;

    [ObservableProperty]
    private string? _equipment;

    [ObservableProperty]
    private bool _isTimeBased;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private string? _instructions;

    [ObservableProperty]
    private string? _notes;

    public ObservableCollection<MuscleSelection> SelectedMuscles { get; } = [];

    public bool HasMuscles => SelectedMuscles.Count > 0;

    public void Receive(MuscleSelectionMessage message)
    {
        SelectedMuscles.Clear();
        foreach (var m in message.Value)
            SelectedMuscles.Add(m);

        OnPropertyChanged(nameof(HasMuscles));
    }

    [RelayCommand]
    private void RemoveMuscle(MuscleSelection muscle)
    {
        SelectedMuscles.Remove(muscle);
        OnPropertyChanged(nameof(HasMuscles));
    }

    [RelayCommand]
    private async Task GoToSelectMuscles()
    {
        // Store current selections so SelectMusclesPage can pre-select them
        SelectMusclesViewModel.PreviousSelections = SelectedMuscles.ToList();
        await Shell.Current.GoToAsync(nameof(SelectMusclesPage));
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlertAsync("Validation", "Exercise name is required.", "OK");
            return;
        }

        var exercise = new Exercise
        {
            Name = Name.Trim(),
            ExerciseType = ExerciseType?.ToLower(),
            Equipment = Equipment,
            IsTimeBased = IsTimeBased,
            Description = Description,
            Instructions = Instructions,
            Notes = Notes
        };

        var id = await _db.SaveExerciseAsync(exercise);

        if (SelectedMuscles.Count > 0)
            await _db.SaveExerciseMusclesAsync(id, SelectedMuscles.ToList());

        WeakReferenceMessenger.Default.Unregister<MuscleSelectionMessage>(this);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        WeakReferenceMessenger.Default.Unregister<MuscleSelectionMessage>(this);
        await Shell.Current.GoToAsync("..");
    }
}
