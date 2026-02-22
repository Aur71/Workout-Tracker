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

    private readonly DataTransferService _transfer;

    private readonly LoadingService _loading;

    public ProgramListViewModel(DatabaseService db, DataTransferService transfer, LoadingService loading)
    {
        _db = db;
        _transfer = transfer;
        _loading = loading;
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
        await _loading.RunAsync(async () =>
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
        }, "Loading...");
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

    [RelayCommand]
    private async Task ImportProgram()
    {
        try
        {
            var fileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "application/json" } },
                { DevicePlatform.iOS, new[] { "public.json" } },
                { DevicePlatform.WinUI, new[] { ".json" } },
                { DevicePlatform.macOS, new[] { "public.json" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select Program File",
                FileTypes = fileTypes
            });

            if (result == null) return;

            int newProgramId;

            await _loading.RunAsync(async () =>
            {
                using var stream = await result.OpenReadAsync();
                newProgramId = await _transfer.ImportProgramAsync(stream);
            }, "Importing...");

            await Shell.Current.DisplayAlertAsync(
                "Import Complete",
                "Program has been imported successfully.",
                "OK");

            await LoadProgramsAsync();
        }
        catch (InvalidOperationException ex)
        {
            await Shell.Current.DisplayAlertAsync("Import Failed", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Import Failed", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }
}
