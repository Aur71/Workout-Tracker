namespace Workout_Tracker.View;

public partial class ProgramDetailPage : ContentPage
{
    public ProgramDetailPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnEditTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(NewProgramPage)}?edit=true");
    }
}
