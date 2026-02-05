namespace Workout_Tracker.View;

public partial class ExerciseListPage : ContentPage
{
    public ExerciseListPage()
    {
        InitializeComponent();
    }

    private async void OnAddExerciseTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NewExercisePage));
    }

    private async void OnExerciseTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ExerciseDetailPage));
    }
}
