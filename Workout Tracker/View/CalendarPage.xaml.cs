using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class CalendarPage : ContentPage
{
    private readonly CalendarViewModel _vm;

    public CalendarPage(CalendarViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Content.Opacity = 0;
        await _vm.LoadMonthAsync();
        await Content.FadeTo(1, 250, Easing.CubicOut);
    }
}
