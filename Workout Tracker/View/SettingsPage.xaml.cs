using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Content.Opacity = 0;
        await Content.FadeTo(1, 250, Easing.CubicOut);
    }
}
