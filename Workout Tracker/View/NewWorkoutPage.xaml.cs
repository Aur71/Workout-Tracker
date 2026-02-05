namespace Workout_Tracker.View;

public partial class NewWorkoutPage : ContentPage, IQueryAttributable
{
    public NewWorkoutPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("edit", out var editValue) && editValue?.ToString() == "true")
        {
            PageTitle.Text = "Edit Workout";
        }
    }

    private async void OnCancelTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnAddExercisesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddExercisesPage));
    }
}
