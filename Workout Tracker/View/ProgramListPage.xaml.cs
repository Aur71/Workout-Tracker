namespace Workout_Tracker.View;

public partial class ProgramListPage : ContentPage
{
    public ProgramListPage()
    {
        InitializeComponent();
    }

    private async void OnAddProgramTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NewProgramPage));
    }

    private async void OnProgramTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ProgramDetailPage));
    }
}
