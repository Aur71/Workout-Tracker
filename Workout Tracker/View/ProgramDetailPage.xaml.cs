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
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Content.Opacity = 0;
        if (query.TryGetValue("id", out var idValue) && int.TryParse(idValue?.ToString(), out int id))
        {
            _programId = id;
            await _vm.LoadProgramAsync(id);
        }
        await Content.FadeTo(1, 250, Easing.CubicOut);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_programId.HasValue && _vm.Program != null)
        {
            Content.Opacity = 0;
            await _vm.LoadProgramAsync(_programId.Value);
            await Content.FadeTo(1, 250, Easing.CubicOut);
        }
    }
}
