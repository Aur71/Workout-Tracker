using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class AnalyticsPage : ContentPage
{
    private readonly AnalyticsViewModel _vm;

    public AnalyticsPage(AnalyticsViewModel vm)
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
