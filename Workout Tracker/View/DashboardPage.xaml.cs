using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _vm;

    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Content.Opacity = 0;
        await _vm.LoadAsync();
        await Content.FadeTo(1, 250, Easing.CubicOut);
    }
}
