using Workout_Tracker.Extensions;
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
        this.AddLoadingOverlay();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadExercisesAsync();
    }
}
