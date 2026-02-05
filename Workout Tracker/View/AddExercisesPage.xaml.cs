namespace Workout_Tracker.View;

public partial class AddExercisesPage : ContentPage
{
    public AddExercisesPage()
    {
        InitializeComponent();
    }

    private async void OnCancelTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
