using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class NewExercisePage : ContentPage, IQueryAttributable
{
    private readonly NewExerciseViewModel _vm;

    public NewExercisePage(NewExerciseViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.AddLoadingOverlay();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("edit", out var editValue) && editValue?.ToString() == "true")
        {
            PageTitle.Text = "Edit Exercise";

            if (query.TryGetValue("id", out var idValue) && int.TryParse(idValue?.ToString(), out int id))
            {
                await _vm.LoadExerciseAsync(id);
            }
        }
    }
}
