using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Model;
using Workout_Tracker.Services;
using Workout_Tracker.View;

namespace Workout_Tracker.ViewModel;

public partial class ProgramDetailViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public ProgramDetailViewModel(DatabaseService db)
    {
        _db = db;
    }

    [ObservableProperty]
    private ProgramDisplay? _program;

    public ObservableCollection<SessionDisplay> Sessions { get; } = [];

    [ObservableProperty]
    private int _totalSessions;

    [ObservableProperty]
    private int _plannedSessions;

    public bool HasGoal => Program?.HasGoal == true;
    public bool HasNotes => Program?.HasNotes == true;
    public bool HasDuration => Program?.HasDuration == true;
    public bool HasSessions => Sessions.Count > 0;

    public async Task LoadProgramAsync(int id)
    {
        var p = await _db.GetProgramByIdAsync(id);
        if (p == null) return;

        Program = new ProgramDisplay
        {
            Id = p.Id,
            Name = p.Name,
            Goal = p.Goal,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Notes = p.Notes,
            Color = p.Color
        };

        OnPropertyChanged(nameof(HasGoal));
        OnPropertyChanged(nameof(HasNotes));
        OnPropertyChanged(nameof(HasDuration));

        await LoadSessionsAsync(id);
    }

    private async Task LoadSessionsAsync(int programId)
    {
        var sessions = await _db.GetSessionsForProgramAsync(programId);
        Sessions.Clear();
        foreach (var s in sessions)
            Sessions.Add(s);

        TotalSessions = Sessions.Count;
        PlannedSessions = Sessions.Count(s => s.ExerciseCount > 0);
        OnPropertyChanged(nameof(HasSessions));
    }

    [RelayCommand]
    private async Task AddSession()
    {
        if (Program == null) return;
        await Shell.Current.GoToAsync($"{nameof(NewSessionPage)}?programId={Program.Id}");
    }

    [RelayCommand]
    private async Task OpenSession(SessionDisplay session)
    {
        if (session.IsCompleted)
            await Shell.Current.GoToAsync($"{nameof(ActiveWorkoutPage)}?sessionId={session.Id}");
        else
            await Shell.Current.GoToAsync($"{nameof(NewSessionPage)}?edit=true&id={session.Id}");
    }

    [RelayCommand]
    private async Task DeleteSession(SessionDisplay session)
    {
        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Delete Session",
            $"Delete session on {session.DateDisplay}?",
            "Delete", "Cancel");

        if (!confirm) return;

        await _db.DeleteSessionAsync(session.Id);
        if (Program != null)
            await LoadSessionsAsync(Program.Id);
    }

    [RelayCommand]
    private async Task Edit()
    {
        if (Program == null) return;
        await Shell.Current.GoToAsync($"{nameof(NewProgramPage)}?edit=true&id={Program.Id}");
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (Program == null) return;

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Delete Program",
            $"Are you sure you want to delete \"{Program.Name}\"?",
            "Delete", "Cancel");

        if (!confirm) return;

        await _db.DeleteProgramAsync(Program.Id);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Back()
    {
        await Shell.Current.GoToAsync("..");
    }
}
