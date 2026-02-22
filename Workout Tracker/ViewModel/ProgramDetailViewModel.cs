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

    private readonly DataTransferService _transfer;

    private readonly LoadingService _loading;

    public ProgramDetailViewModel(DatabaseService db, DataTransferService transfer, LoadingService loading)
    {
        _db = db;
        _transfer = transfer;
        _loading = loading;
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
        await _loading.RunAsync(async () =>
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
        }, "Loading...");
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
    private async Task DuplicateSession(SessionDisplay session)
    {
        await Shell.Current.GoToAsync($"{nameof(NewSessionPage)}?duplicate=true&id={session.Id}");
    }

    [RelayCommand]
    private async Task DeleteSession(SessionDisplay session)
    {
        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Delete Session",
            $"Delete session on {session.DateDisplay}?",
            "Delete", "Cancel");

        if (!confirm) return;

        await _loading.RunAsync(async () =>
        {
            await _db.DeleteSessionAsync(session.Id);
            if (Program != null)
                await LoadSessionsAsync(Program.Id);
        }, "Deleting...");
    }

    [RelayCommand]
    private async Task Duplicate()
    {
        if (Program == null) return;

        string? newName = await Shell.Current.DisplayPromptAsync(
            "Duplicate Program",
            "Enter a name for the new program:",
            initialValue: $"{Program.Name} (Copy)",
            maxLength: 200);

        if (newName == null) return; // cancelled

        if (string.IsNullOrWhiteSpace(newName))
        {
            await Shell.Current.DisplayAlertAsync("Invalid Name", "Program name cannot be empty.", "OK");
            return;
        }

        await _loading.RunAsync(async () =>
        {
            int newId = await _db.DuplicateProgramAsync(Program.Id, newName.Trim(), DateTime.Today);

            if (newId < 0)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Could not duplicate program. Source program not found.", "OK");
                return;
            }

            await Shell.Current.GoToAsync($"../{nameof(ProgramDetailPage)}?id={newId}");
        }, "Duplicating...");
    }

    [RelayCommand]
    private async Task ExportProgram()
    {
        if (Program == null) return;

        try
        {
            await _loading.RunAsync(async () =>
            {
                var filePath = await _transfer.ExportProgramAsync(Program.Id);

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = $"Share {Program.Name}",
                    File = new ShareFile(filePath)
                });
            }, "Exporting...");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Export Failed", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task Edit()
    {
        if (Program == null) return;
        await Shell.Current.GoToAsync($"{nameof(NewProgramPage)}?edit=true&id={Program.Id}");
    }

    [RelayCommand]
    private async Task ResetCompletion()
    {
        if (Program == null) return;

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Reset Progress",
            "This will reset all session progress. Performed reps, weights, and completion status will be cleared. Continue?",
            "Reset", "Cancel");

        if (!confirm) return;

        await _loading.RunAsync(async () =>
        {
            await _db.ResetProgramCompletionAsync(Program.Id);
            await LoadSessionsAsync(Program.Id);
        }, "Resetting...");
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

        await _loading.RunAsync(async () =>
        {
            await _db.DeleteProgramAsync(Program.Id);
            await Shell.Current.GoToAsync("..");
        }, "Deleting...");
    }

    [RelayCommand]
    private async Task Back()
    {
        await Shell.Current.GoToAsync("..");
    }
}
