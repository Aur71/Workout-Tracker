using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Workout_Tracker.Messages;
using Workout_Tracker.Model;
using Workout_Tracker.Services;
using Workout_Tracker.View;

namespace Workout_Tracker.ViewModel;

public partial class NewSessionViewModel : ObservableObject, IRecipient<ExerciseSelectionMessage>
{
    private readonly DatabaseService _db;
    private int _programId;
    private int? _editSessionId;
    private Session? _originalSession;

    private readonly LoadingService _loading;

    public NewSessionViewModel(DatabaseService db, LoadingService loading)
    {
        _db = db;
        _loading = loading;
        WeakReferenceMessenger.Default.Register(this);
    }

    [ObservableProperty]
    private DateTime _sessionDate = DateTime.Today;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _newTagText = "";

    public ObservableCollection<string> Tags { get; } = [];

    public ObservableCollection<SessionExerciseDisplay> Exercises { get; } = [];

    public bool HasExercises => Exercises.Count > 0;

    public async Task InitAsync(int programId)
    {
        _programId = programId;

        await _loading.RunAsync(async () =>
        {
            // Smart default date
            var lastDate = await _db.GetLastSessionDateForProgramAsync(programId);
            if (lastDate.HasValue)
            {
                SessionDate = lastDate.Value.AddDays(1);
            }
            else
            {
                var program = await _db.GetProgramByIdAsync(programId);
                if (program != null)
                    SessionDate = program.StartDate;
            }
        }, "Loading...");
    }

    public async Task LoadSessionAsync(int sessionId)
    {
        if (_editSessionId.HasValue) return;
        _editSessionId = sessionId;

        await _loading.RunAsync(async () =>
        {
            _originalSession = await _db.GetSessionByIdAsync(sessionId);
            if (_originalSession == null) return;

            _programId = _originalSession.ProgramId ?? 0;
            SessionDate = _originalSession.Date;
            Notes = _originalSession.Notes;

            var exercises = await _db.GetSessionExercisesWithSetsAsync(sessionId);
            Exercises.Clear();
            foreach (var ex in exercises)
                Exercises.Add(ex);
            OnPropertyChanged(nameof(HasExercises));

            var tags = await _db.GetTagsForSessionAsync(sessionId);
            Tags.Clear();
            foreach (var tag in tags)
                Tags.Add(tag);
        }, "Loading...");
    }

    public async Task DuplicateSessionAsync(int sessionId)
    {
        await _loading.RunAsync(async () =>
        {
            var session = await _db.GetSessionByIdAsync(sessionId);
            if (session == null) return;
            _originalSession = null; // Not editing — duplicating

            _programId = session.ProgramId ?? 0;
            Notes = session.Notes;

            // Date = last session in program + 1 day
            var lastDate = await _db.GetLastSessionDateForProgramAsync(_programId);
            SessionDate = lastDate.HasValue ? lastDate.Value.AddDays(1) : session.Date.AddDays(1);

            var exercises = await _db.GetSessionExercisesWithSetsAsync(sessionId);
            Exercises.Clear();
            foreach (var ex in exercises)
                Exercises.Add(ex);
            OnPropertyChanged(nameof(HasExercises));

            var tags = await _db.GetTagsForSessionAsync(sessionId);
            Tags.Clear();
            foreach (var tag in tags)
                Tags.Add(tag);
        }, "Loading...");
    }

    public void Receive(ExerciseSelectionMessage message)
    {
        // Convert ExerciseSelection list to SessionExerciseDisplay list
        var startOrder = Exercises.Count + 1;
        foreach (var sel in message.Value)
        {
            if (!sel.IsSelected) continue;
            // Skip if already added
            if (Exercises.Any(e => e.ExerciseId == sel.Exercise.Id)) continue;

            Exercises.Add(new SessionExerciseDisplay
            {
                ExerciseId = sel.Exercise.Id,
                ExerciseName = sel.Exercise.Name,
                ExerciseType = sel.Exercise.ExerciseType,
                PrimaryMuscle = sel.Exercise.PrimaryMuscle,
                IsTimeBased = sel.Exercise.IsTimeBased,
                Order = startOrder++,
                RestSecondsText = "120"
            });
        }
        OnPropertyChanged(nameof(HasExercises));
    }

    [RelayCommand]
    private void AddTag()
    {
        var tag = NewTagText?.Trim();
        if (!string.IsNullOrWhiteSpace(tag) && !Tags.Contains(tag))
        {
            Tags.Add(tag);
            NewTagText = "";
        }
    }

    [RelayCommand]
    private void RemoveTag(string tag)
    {
        Tags.Remove(tag);
    }

    [RelayCommand]
    private async Task GoToAddExercises()
    {
        // Set previous selections so AddExercisesPage can pre-check them
        AddExercisesViewModel.PreviousSelections = Exercises.Select(e => new ExerciseSelection
        {
            Exercise = new ExerciseDisplay { Id = e.ExerciseId, Name = e.ExerciseName },
            IsSelected = true
        }).ToList();
        await Shell.Current.GoToAsync(nameof(AddExercisesPage));
    }

    [RelayCommand]
    private void RemoveExercise(SessionExerciseDisplay exercise)
    {
        Exercises.Remove(exercise);
        // Re-order
        for (int i = 0; i < Exercises.Count; i++)
            Exercises[i].Order = i + 1;
        OnPropertyChanged(nameof(HasExercises));
    }

    [RelayCommand]
    private void MoveUp(SessionExerciseDisplay exercise)
    {
        var index = Exercises.IndexOf(exercise);
        if (index <= 0) return;
        Exercises.Move(index, index - 1);
        // Re-order
        for (int i = 0; i < Exercises.Count; i++)
            Exercises[i].Order = i + 1;
    }

    [RelayCommand]
    private void MoveDown(SessionExerciseDisplay exercise)
    {
        var index = Exercises.IndexOf(exercise);
        if (index < 0 || index >= Exercises.Count - 1) return;
        Exercises.Move(index, index + 1);
        // Re-order
        for (int i = 0; i < Exercises.Count; i++)
            Exercises[i].Order = i + 1;
    }

    [RelayCommand]
    private void AddSet(SessionExerciseDisplay exercise)
    {
        var nextNumber = exercise.Sets.Count + 1;
        exercise.Sets.Add(new SetDisplay { SetNumber = nextNumber });
    }

    [RelayCommand]
    private void RemoveSet(SetDisplay set)
    {
        // Find the parent exercise
        var parent = Exercises.FirstOrDefault(e => e.Sets.Contains(set));
        if (parent == null) return;
        parent.Sets.Remove(set);
        // Renumber
        int num = 1;
        foreach (var s in parent.Sets)
            s.SetNumber = num++;
    }

    [RelayCommand]
    private void ToggleExpand(SessionExerciseDisplay exercise)
    {
        exercise.IsExpanded = !exercise.IsExpanded;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
        await _loading.RunAsync(async () =>
        {
            var session = new Session
            {
                ProgramId = _programId,
                Date = SessionDate,
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
                // Preserve original metadata when editing
                Week = _originalSession?.Week,
                Day = _originalSession?.Day,
                IsCompleted = _originalSession?.IsCompleted ?? false,
                StartTime = _originalSession?.StartTime,
                EndTime = _originalSession?.EndTime,
                EnergyLevel = _originalSession?.EnergyLevel
            };

            if (_editSessionId.HasValue)
            {
                session.Id = _editSessionId.Value;
                await _db.UpdateSessionAsync(session);
                await _db.DeleteSessionExercisesAndSetsAsync(session.Id);
                if (Exercises.Count > 0)
                    await _db.SaveSessionExercisesWithSetsAsync(session.Id, Exercises.ToList());
                await _db.SaveTagsForSessionAsync(session.Id, Tags.ToList());
            }
            else
            {
                var id = await _db.SaveSessionAsync(session);
                if (Exercises.Count > 0)
                    await _db.SaveSessionExercisesWithSetsAsync(id, Exercises.ToList());
                await _db.SaveTagsForSessionAsync(id, Tags.ToList());
            }

            WeakReferenceMessenger.Default.Unregister<ExerciseSelectionMessage>(this);
            await Shell.Current.GoToAsync("..");
        }, "Saving...");
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        WeakReferenceMessenger.Default.Unregister<ExerciseSelectionMessage>(this);
        await Shell.Current.GoToAsync("..");
    }
}
