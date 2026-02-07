using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class SelectMusclesPage : ContentPage
{
    private readonly SelectMusclesViewModel _vm;

    public SelectMusclesPage(SelectMusclesViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadMusclesAsync();
    }
}
