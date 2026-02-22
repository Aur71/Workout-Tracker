using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Services;
using Program = Workout_Tracker.Model.Program;

namespace Workout_Tracker.ViewModel;

public partial class NewProgramViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private int? _editProgramId;

    private readonly LoadingService _loading;

    public NewProgramViewModel(DatabaseService db, LoadingService loading)
    {
        _db = db;
        _loading = loading;
    }

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _goal;

    [ObservableProperty]
    private DateTime _startDate = DateTime.Today;

    [ObservableProperty]
    private DateTime? _endDate;

    [ObservableProperty]
    private bool _hasEndDate;

    [ObservableProperty]
    private DateTime _endDateValue = DateTime.Today.AddDays(42);

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private string _selectedColor = "#00D9A5";

    [ObservableProperty]
    private bool _isBusy;

    public List<string> ColorOptions { get; } =
    [
        "#00D9A5", // Primary/Teal
        "#FF6B5B", // Secondary/Coral
        "#4A6CF7", // Tertiary/Blue
        "#10B981", // Success/Green
        "#F59E0B", // Warning/Amber
        "#8A8A8A"  // Gray
    ];

    partial void OnHasEndDateChanged(bool value)
    {
        if (!value)
            EndDateValue = DateTime.Today.AddDays(42);
    }

    private DateTime? _originalStartDate;

    public async Task LoadProgramAsync(int id)
    {
        if (_editProgramId.HasValue) return;
        _editProgramId = id;

        var program = await _db.GetProgramByIdAsync(id);
        if (program == null) return;

        Name = program.Name;
        Goal = program.Goal;
        StartDate = program.StartDate;
        _originalStartDate = program.StartDate;
        Notes = program.Notes;
        SelectedColor = program.Color ?? "#00D9A5";

        if (program.EndDate.HasValue)
        {
            HasEndDate = true;
            EndDateValue = program.EndDate.Value;
        }
    }

    [RelayCommand]
    private void SelectColor(string color)
    {
        SelectedColor = color;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlertAsync("Validation", "Program name is required.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
        await _loading.RunAsync(async () =>
        {
            var program = new Program
            {
                Name = Name.Trim(),
                Goal = Goal,
                StartDate = StartDate,
                EndDate = HasEndDate ? EndDateValue : null,
                Notes = Notes,
                Color = SelectedColor
            };

            if (_editProgramId.HasValue)
            {
                program.Id = _editProgramId.Value;
                await _db.UpdateProgramAsync(program);

                if (_originalStartDate.HasValue && StartDate.Date != _originalStartDate.Value.Date)
                {
                    var offset = StartDate.Date - _originalStartDate.Value.Date;
                    await _db.OffsetSessionDatesAsync(_editProgramId.Value, offset);
                }
            }
            else
            {
                await _db.SaveProgramAsync(program);
            }

            await Shell.Current.GoToAsync("..");
        }, "Saving...");
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
