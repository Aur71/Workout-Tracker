using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class WorkoutListPage : ContentPage
{
    private readonly WorkoutListViewModel _vm;

    public WorkoutListPage(WorkoutListViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Content.Opacity = 0;
        await _vm.LoadWorkoutsAsync();
        await Content.FadeTo(1, 250, Easing.CubicOut);
    }
}
