namespace Workout_Tracker.View;

public partial class WorkoutDetailPage : ContentPage
{
    public WorkoutDetailPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnEditTapped(object sender, TappedEventArgs e)
    {
        // Navigate to NewWorkoutPage in edit mode
        // Later we'll also pass the workout ID as a parameter
        await Shell.Current.GoToAsync($"{nameof(NewWorkoutPage)}?edit=true");
    }
}
