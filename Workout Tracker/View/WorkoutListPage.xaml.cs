namespace Workout_Tracker.View;

public partial class WorkoutListPage : ContentPage
{
    public WorkoutListPage()
    {
        InitializeComponent();
    }

    private async void OnAddWorkoutTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NewWorkoutPage));
    }

    private async void OnWorkoutTapped(object sender, TappedEventArgs e)
    {
        // Later we'll pass the workout ID as a parameter
        await Shell.Current.GoToAsync(nameof(WorkoutDetailPage));
    }
}
