using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class CalendarPage : ContentPage
{
    private readonly CalendarViewModel _vm;

    public CalendarPage(CalendarViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.AddLoadingOverlay();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try { await _vm.LoadMonthAsync(); }
        catch (Exception ex) { await DisplayAlert("Error", ex.Message, "OK"); }
    }
}
