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
        await _vm.LoadWorkoutsAsync();
    }
}
