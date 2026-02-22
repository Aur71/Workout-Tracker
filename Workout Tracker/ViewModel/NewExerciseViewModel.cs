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
    private int? _editExerciseId;

    private readonly LoadingService _loading;

    public NewExerciseViewModel(DatabaseService db, LoadingService loading)
    {
        _db = db;
        _loading = loading;
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

    [ObservableProperty]
    private bool _isBusy;

    public ObservableCollection<MuscleSelection> SelectedMuscles { get; } = [];

    public bool HasMuscles => SelectedMuscles.Count > 0;

    public async Task LoadExerciseAsync(int id)
    {
        if (_editExerciseId.HasValue) return;
        _editExerciseId = id;

        var display = await _db.GetExerciseByIdAsync(id);
        if (display == null) return;

        Name = display.Name;
        ExerciseType = display.TypeDisplay;
        Equipment = display.Equipment;
        IsTimeBased = display.IsTimeBased;
        Description = display.Description;
        Instructions = display.Instructions;
        Notes = display.Notes;

        // Load muscles as MuscleSelection objects
        var allMuscles = await _db.GetAllMusclesAsync();
        SelectedMuscles.Clear();
        foreach (var md in display.Muscles)
        {
            var muscle = allMuscles.FirstOrDefault(m => m.Name == md.Name);
            if (muscle != null)
            {
                SelectedMuscles.Add(new MuscleSelection
                {
                    Muscle = muscle,
                    IsSelected = true,
                    Role = md.Role
                });
            }
        }
        OnPropertyChanged(nameof(HasMuscles));
    }

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
        SelectMusclesViewModel.PreviousSelections = SelectedMuscles.ToList();
        await Shell.Current.GoToAsync(nameof(SelectMusclesPage));
    }

    [RelayCommand]
    private async Task Save()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlertAsync("Validation", "Exercise name is required.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
        await _loading.RunAsync(async () =>
        {
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

            if (_editExerciseId.HasValue)
            {
                exercise.Id = _editExerciseId.Value;
                await _db.UpdateExerciseAsync(exercise);
                await _db.DeleteExerciseMusclesAsync(exercise.Id);
                if (SelectedMuscles.Count > 0)
                    await _db.SaveExerciseMusclesAsync(exercise.Id, SelectedMuscles.ToList());
            }
            else
            {
                var id = await _db.SaveExerciseAsync(exercise);
                if (SelectedMuscles.Count > 0)
                    await _db.SaveExerciseMusclesAsync(id, SelectedMuscles.ToList());
            }

            WeakReferenceMessenger.Default.Unregister<MuscleSelectionMessage>(this);
            await Shell.Current.GoToAsync("..");
        }, "Saving...");
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        WeakReferenceMessenger.Default.Unregister<MuscleSelectionMessage>(this);
        await Shell.Current.GoToAsync("..");
    }
}
