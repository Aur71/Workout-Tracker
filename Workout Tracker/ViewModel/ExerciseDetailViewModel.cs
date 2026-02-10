using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Model;
using Workout_Tracker.Services;
using Workout_Tracker.View;

namespace Workout_Tracker.ViewModel;

public partial class ExerciseDetailViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    private readonly LoadingService _loading;

    public ExerciseDetailViewModel(DatabaseService db, LoadingService loading)
    {
        _db = db;
        _loading = loading;
    }

    [ObservableProperty]
    private ExerciseDisplay? _exercise;

    public bool HasDescription => !string.IsNullOrWhiteSpace(Exercise?.Description);
    public bool HasInstructions => !string.IsNullOrWhiteSpace(Exercise?.Instructions);
    public bool HasNotes => !string.IsNullOrWhiteSpace(Exercise?.Notes);
    public bool HasMuscles => Exercise?.Muscles.Count > 0;

    public async Task LoadExerciseAsync(int id)
    {
        await _loading.RunAsync(async () =>
        {
            Exercise = await _db.GetExerciseByIdAsync(id);
            OnPropertyChanged(nameof(HasDescription));
            OnPropertyChanged(nameof(HasInstructions));
            OnPropertyChanged(nameof(HasNotes));
            OnPropertyChanged(nameof(HasMuscles));
        }, "Loading...");
    }

    [RelayCommand]
    private async Task Edit()
    {
        if (Exercise == null) return;
        await Shell.Current.GoToAsync($"{nameof(NewExercisePage)}?edit=true&id={Exercise.Id}");
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (Exercise == null) return;

        var workoutCount = await _db.GetWorkoutCountForExerciseAsync(Exercise.Id);
        var message = workoutCount > 0
            ? $"Are you sure you want to delete \"{Exercise.Name}\"?\n\nThis exercise is used in {workoutCount} workout template{(workoutCount != 1 ? "s" : "")}. It will be removed from those workouts."
            : $"Are you sure you want to delete \"{Exercise.Name}\"?";

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Delete Exercise", message, "Delete", "Cancel");

        if (!confirm) return;

        await _loading.RunAsync(async () =>
        {
            if (workoutCount > 0)
                await _db.DeleteExerciseFromWorkoutsAsync(Exercise.Id);

            await _db.DeleteExerciseAsync(Exercise.Id);
            await Shell.Current.GoToAsync("..");
        }, "Deleting...");
    }

    [RelayCommand]
    private async Task Back()
    {
        await Shell.Current.GoToAsync("..");
    }
}
