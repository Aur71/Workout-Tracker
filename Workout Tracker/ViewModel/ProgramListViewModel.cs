using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Model;
using Workout_Tracker.Services;
using Workout_Tracker.View;

namespace Workout_Tracker.ViewModel;

public partial class ProgramListViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public ProgramListViewModel(DatabaseService db)
    {
        _db = db;
    }

    public ObservableCollection<ProgramDisplay> ActivePrograms { get; } = [];
    public ObservableCollection<ProgramDisplay> UpcomingPrograms { get; } = [];
    public ObservableCollection<ProgramDisplay> CompletedPrograms { get; } = [];

    [ObservableProperty]
    private bool _hasActive;

    [ObservableProperty]
    private bool _hasUpcoming;

    [ObservableProperty]
    private bool _hasCompleted;

    [ObservableProperty]
    private bool _isEmpty;

    public async Task LoadProgramsAsync()
    {
        var programs = await _db.GetAllProgramsAsync();
        var today = DateTime.Today;

        var displays = programs.Select(p => new ProgramDisplay
        {
            Id = p.Id,
            Name = p.Name,
            Goal = p.Goal,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Notes = p.Notes,
            Color = p.Color
        }).ToList();

        ActivePrograms.Clear();
        UpcomingPrograms.Clear();
        CompletedPrograms.Clear();

        foreach (var p in displays)
        {
            if (p.StartDate <= today && (!p.EndDate.HasValue || p.EndDate.Value >= today))
                ActivePrograms.Add(p);
            else if (p.StartDate > today)
                UpcomingPrograms.Add(p);
            else
                CompletedPrograms.Add(p);
        }

        HasActive = ActivePrograms.Count > 0;
        HasUpcoming = UpcomingPrograms.Count > 0;
        HasCompleted = CompletedPrograms.Count > 0;
        IsEmpty = !HasActive && !HasUpcoming && !HasCompleted;
    }

    [RelayCommand]
    private async Task GoToDetail(ProgramDisplay program)
    {
        await Shell.Current.GoToAsync($"{nameof(ProgramDetailPage)}?id={program.Id}");
    }

    [RelayCommand]
    private async Task GoToAdd()
    {
        await Shell.Current.GoToAsync(nameof(NewProgramPage));
    }
}
