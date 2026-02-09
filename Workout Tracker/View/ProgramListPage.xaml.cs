using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class ProgramListPage : ContentPage
{
    private readonly ProgramListViewModel _vm;

    public ProgramListPage(ProgramListViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Content.Opacity = 0;
        await _vm.LoadProgramsAsync();
        await Content.FadeTo(1, 250, Easing.CubicOut);
    }
}
