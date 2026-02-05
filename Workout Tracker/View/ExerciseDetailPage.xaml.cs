namespace Workout_Tracker.View;

public partial class ExerciseDetailPage : ContentPage
{
    public ExerciseDetailPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnEditTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(NewExercisePage)}?edit=true");
    }
}
