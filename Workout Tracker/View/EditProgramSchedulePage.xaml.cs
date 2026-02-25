using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class EditProgramSchedulePage : ContentPage, IQueryAttributable
{
    private readonly EditProgramScheduleViewModel _vm;

    public EditProgramSchedulePage(EditProgramScheduleViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.AddLoadingOverlay();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        try
        {
            if (query.TryGetValue("programId", out var idValue) && int.TryParse(idValue?.ToString(), out int programId))
                await _vm.LoadAsync(programId);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
