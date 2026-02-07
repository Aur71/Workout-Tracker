using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Model;
using Workout_Tracker.Services;
using Workout_Tracker.View;

namespace Workout_Tracker.ViewModel;

public partial class WorkoutDetailViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public WorkoutDetailViewModel(DatabaseService db)
    {
        _db = db;
    }

    [ObservableProperty]
    private WorkoutDisplay? _workout;

    public bool HasNotes => !string.IsNullOrWhiteSpace(Workout?.Notes);
    public bool HasExercises => Workout?.Exercises.Count > 0;

    public async Task LoadWorkoutAsync(int id)
    {
        Workout = await _db.GetWorkoutByIdAsync(id);
        OnPropertyChanged(nameof(HasNotes));
        OnPropertyChanged(nameof(HasExercises));
    }

    [RelayCommand]
    private async Task Edit()
    {
        if (Workout == null) return;
        await Shell.Current.GoToAsync($"{nameof(NewWorkoutPage)}?edit=true&id={Workout.Id}");
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (Workout == null) return;

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Delete Workout",
            $"Are you sure you want to delete \"{Workout.Name}\"?",
            "Delete", "Cancel");

        if (!confirm) return;

        await _db.DeleteWorkoutAsync(Workout.Id);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Back()
    {
        await Shell.Current.GoToAsync("..");
    }
}
