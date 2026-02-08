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
        await _vm.LoadLogsAsync();
    }
}
