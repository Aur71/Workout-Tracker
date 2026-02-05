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
}
