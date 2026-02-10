using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class NewWorkoutPage : ContentPage, IQueryAttributable
{
    private readonly NewWorkoutViewModel _vm;

    public NewWorkoutPage(NewWorkoutViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        this.AddLoadingOverlay();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("edit", out var editValue) && editValue?.ToString() == "true")
        {
            PageTitle.Text = "Edit Workout";

            if (query.TryGetValue("id", out var idValue) && int.TryParse(idValue?.ToString(), out var id))
            {
                await _vm.LoadWorkoutAsync(id);
            }
        }
    }
}
