using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class LogListPage : ContentPage
{
    private readonly LogListViewModel _vm;

    public LogListPage(LogListViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Content.Opacity = 0;
        await _vm.LoadLogsAsync();
        await Content.FadeTo(1, 250, Easing.CubicOut);
    }
}
