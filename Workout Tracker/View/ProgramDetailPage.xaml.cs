using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class ProgramDetailPage : ContentPage, IQueryAttributable
{
    private readonly ProgramDetailViewModel _vm;
    private int? _programId;

    public ProgramDetailPage(ProgramDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.AddLoadingOverlay();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var idValue) && int.TryParse(idValue?.ToString(), out int id))
        {
            _programId = id;
            await _vm.LoadProgramAsync(id);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_programId.HasValue && _vm.Program != null)
        {
            await _vm.LoadProgramAsync(_programId.Value);
        }
    }
}
