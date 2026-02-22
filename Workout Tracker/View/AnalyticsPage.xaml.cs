using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class AnalyticsPage : ContentPage
{
    private readonly AnalyticsViewModel _vm;

    public AnalyticsPage(AnalyticsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.AddLoadingOverlay();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try { await _vm.LoadAsync(); }
        catch (Exception ex) { await DisplayAlert("Error", ex.Message, "OK"); }
    }
}
