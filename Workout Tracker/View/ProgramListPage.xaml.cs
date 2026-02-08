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
        await _vm.LoadProgramsAsync();
    }
}
