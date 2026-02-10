using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

}
