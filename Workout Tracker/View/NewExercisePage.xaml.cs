namespace Workout_Tracker.View;

public partial class NewExercisePage : ContentPage, IQueryAttributable
{
    public NewExercisePage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("edit", out var editValue) && editValue?.ToString() == "true")
        {
            PageTitle.Text = "Edit Exercise";
        }
    }

    private async void OnCancelTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnSelectMusclesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SelectMusclesPage));
    }
}
