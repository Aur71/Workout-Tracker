namespace Workout_Tracker.View;

public partial class NewProgramPage : ContentPage, IQueryAttributable
{
    public NewProgramPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("edit", out var editValue) && editValue?.ToString() == "true")
        {
            PageTitle.Text = "Edit Program";
        }
    }

    private async void OnCancelTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
