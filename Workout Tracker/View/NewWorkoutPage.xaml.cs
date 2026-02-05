namespace Workout_Tracker.View;

public partial class NewWorkoutPage : ContentPage
{
    public NewWorkoutPage()
    {
        InitializeComponent();
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
