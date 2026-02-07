using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class NewExercisePage : ContentPage, IQueryAttributable
{
    public NewExercisePage(NewExerciseViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("edit", out var editValue) && editValue?.ToString() == "true")
        {
            PageTitle.Text = "Edit Exercise";
        }
    }
}
