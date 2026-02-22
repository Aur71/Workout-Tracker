using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class LogListPage : ContentPage
{
    private readonly LogListViewModel _vm;

    public LogListPage(LogListViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.AddLoadingOverlay();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try { await _vm.LoadLogsAsync(); }
        catch (Exception ex) { await DisplayAlert("Error", ex.Message, "OK"); }
    }
}
