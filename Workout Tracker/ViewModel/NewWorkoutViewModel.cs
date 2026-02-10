using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Workout_Tracker.Messages;
using Workout_Tracker.Model;
using Workout_Tracker.Services;
using Workout_Tracker.View;

namespace Workout_Tracker.ViewModel;

public partial class NewWorkoutViewModel : ObservableObject, IRecipient<ExerciseSelectionMessage>
{
    private readonly DatabaseService _db;
    private int? _editWorkoutId;

    private readonly LoadingService _loading;

    public NewWorkoutViewModel(DatabaseService db, LoadingService loading)
    {
        _db = db;
        _loading = loading;
        WeakReferenceMessenger.Default.Register(this);
    }

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _goal;

    [ObservableProperty]
    private string? _estimatedDurationText;

    [ObservableProperty]
    private string? _notes;

    public ObservableCollection<ExerciseSelection> SelectedExercises { get; } = [];

    public bool HasExercises => SelectedExercises.Count > 0;

    public async Task LoadWorkoutAsync(int id)
    {
        if (_editWorkoutId.HasValue) return;
        _editWorkoutId = id;

        var display = await _db.GetWorkoutByIdAsync(id);
        if (display == null) return;

        Name = display.Name;
        Goal = display.GoalDisplay;
        EstimatedDurationText = display.EstimatedDuration?.ToString();
        Notes = display.Notes;

        // Load exercises as ExerciseSelection objects
        var allExercises = await _db.GetAllExercisesAsync();
        SelectedExercises.Clear();
        foreach (var wed in display.Exercises)
        {
            var exerciseDisplay = allExercises.FirstOrDefault(e => e.Id == wed.ExerciseId);
            if (exerciseDisplay != null)
            {
                SelectedExercises.Add(new ExerciseSelection
                {
                    Exercise = exerciseDisplay,
                    IsSelected = true
                });
            }
        }
        OnPropertyChanged(nameof(HasExercises));
    }

    public void Receive(ExerciseSelectionMessage message)
    {
        SelectedExercises.Clear();
        foreach (var e in message.Value)
            SelectedExercises.Add(e);

        OnPropertyChanged(nameof(HasExercises));
    }

    [RelayCommand]
    private void RemoveExercise(ExerciseSelection exercise)
    {
        SelectedExercises.Remove(exercise);
        OnPropertyChanged(nameof(HasExercises));
    }

    [RelayCommand]
    private async Task GoToAddExercises()
    {
        AddExercisesViewModel.PreviousSelections = SelectedExercises.ToList();
        await Shell.Current.GoToAsync(nameof(AddExercisesPage));
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlertAsync("Validation", "Workout name is required.", "OK");
            return;
        }

        await _loading.RunAsync(async () =>
        {
            int? duration = null;
            if (!string.IsNullOrWhiteSpace(EstimatedDurationText) && int.TryParse(EstimatedDurationText, out var d))
                duration = d;

            var workout = new Workout
            {
                Name = Name.Trim(),
                Goal = Goal?.ToLower(),
                EstimatedDuration = duration,
                Notes = Notes
            };

            if (_editWorkoutId.HasValue)
            {
                workout.Id = _editWorkoutId.Value;
                await _db.UpdateWorkoutAsync(workout);
                await _db.DeleteWorkoutExercisesAsync(workout.Id);
                if (SelectedExercises.Count > 0)
                    await _db.SaveWorkoutExercisesAsync(workout.Id, SelectedExercises.ToList());
            }
            else
            {
                var id = await _db.SaveWorkoutAsync(workout);
                if (SelectedExercises.Count > 0)
                    await _db.SaveWorkoutExercisesAsync(id, SelectedExercises.ToList());
            }

            WeakReferenceMessenger.Default.Unregister<ExerciseSelectionMessage>(this);
            await Shell.Current.GoToAsync("..");
        }, "Saving...");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        WeakReferenceMessenger.Default.Unregister<ExerciseSelectionMessage>(this);
        await Shell.Current.GoToAsync("..");
    }
}
