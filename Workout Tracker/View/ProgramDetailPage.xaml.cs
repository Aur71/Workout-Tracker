using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class ProgramDetailPage : ContentPage, IQueryAttributable
{
    private readonly ProgramDetailViewModel _vm;
    private int? _programId;
    private bool _skipNextAppearing;

    public ProgramDetailPage(ProgramDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.AddLoadingOverlay();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        try
        {
            if (query.TryGetValue("id", out var idValue) && int.TryParse(idValue?.ToString(), out int id))
            {
                _programId = id;
                _skipNextAppearing = true;
                await _vm.LoadProgramAsync(id);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            if (_skipNextAppearing)
            {
                _skipNextAppearing = false;
                return;
            }
            // Reload after returning from sub-pages to show updated data
            if (_programId.HasValue)
                await _vm.LoadProgramAsync(_programId.Value);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
