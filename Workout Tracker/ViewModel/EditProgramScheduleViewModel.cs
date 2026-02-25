using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Model;
using Workout_Tracker.Services;

namespace Workout_Tracker.ViewModel;

public partial class EditProgramScheduleViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly LoadingService _loading;
    private int _programId;

    public EditProgramScheduleViewModel(DatabaseService db, LoadingService loading)
    {
        _db = db;
        _loading = loading;
    }

    public ObservableCollection<ScheduleSessionDisplay> Sessions { get; } = [];

    [ObservableProperty]
    private bool _hasUnsavedChanges;

    public async Task LoadAsync(int programId)
    {
        _programId = programId;
        await _loading.RunAsync(async () =>
        {
            var sessions = await _db.GetSessionsForProgramAsync(programId);
            Sessions.Clear();
            foreach (var s in sessions)
            {
                var display = new ScheduleSessionDisplay
                {
                    Id = s.Id,
                    Date = s.Date,
                    EditDate = s.Date,
                    ExerciseCount = s.ExerciseCount,
                    SetCount = s.SetCount,
                    IsCompleted = s.IsCompleted,
                    Tags = s.Tags
                };
                display.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(ScheduleSessionDisplay.EditDate))
                        HasUnsavedChanges = Sessions.Any(x => x.HasChanged);
                };
                Sessions.Add(display);
            }
            HasUnsavedChanges = false;
        }, "Loading...");
    }

    [RelayCommand]
    private async Task Save()
    {
        await _loading.RunAsync(async () =>
        {
            var updates = Sessions
                .Where(s => s.HasChanged)
                .Select(s => (s.Id, s.EditDate))
                .ToList();

            if (updates.Count > 0)
                await _db.UpdateSessionDatesAsync(updates);

            await LoadAsync(_programId);
        }, "Saving...");
    }

    [RelayCommand]
    private async Task Back()
    {
        if (HasUnsavedChanges)
        {
            bool discard = await Shell.Current.DisplayAlertAsync(
                "Unsaved Changes",
                "You have unsaved date changes. Discard them?",
                "Discard", "Cancel");

            if (!discard) return;
        }

        await Shell.Current.GoToAsync("..");
    }
}
