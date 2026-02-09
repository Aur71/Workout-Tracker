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
        Content.Opacity = 0;
        await _vm.LoadExercisesAsync();
        await Content.FadeTo(1, 250, Easing.CubicOut);
    }
}
