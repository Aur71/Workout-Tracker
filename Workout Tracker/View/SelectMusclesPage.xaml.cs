namespace Workout_Tracker.View;

public partial class SelectMusclesPage : ContentPage
{
    public SelectMusclesPage()
    {
        InitializeComponent();
    }

    private async void OnCancelTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
