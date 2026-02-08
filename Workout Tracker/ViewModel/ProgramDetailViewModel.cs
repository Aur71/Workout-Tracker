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

    public bool HasGoal => Program?.HasGoal == true;
    public bool HasNotes => Program?.HasNotes == true;
    public bool HasDuration => Program?.HasDuration == true;

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
