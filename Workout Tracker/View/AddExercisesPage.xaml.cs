using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class AddExercisesPage : ContentPage
{
    private readonly AddExercisesViewModel _vm;

    public AddExercisesPage(AddExercisesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Content.Opacity = 0;
        await _vm.LoadExercisesAsync();
        await Content.FadeTo(1, 250, Easing.CubicOut);
    }
}
