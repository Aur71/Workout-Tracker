using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class ExerciseListPage : ContentPage
{
    private readonly ExerciseListViewModel _vm;

    public ExerciseListPage(ExerciseListViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadExercisesAsync();
    }
}
