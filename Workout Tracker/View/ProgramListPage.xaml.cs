using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class ProgramListPage : ContentPage
{
    private readonly ProgramListViewModel _vm;

    public ProgramListPage(ProgramListViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.AddLoadingOverlay();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try { await _vm.LoadProgramsAsync(); }
        catch (Exception ex) { await DisplayAlert("Error", ex.Message, "OK"); }
    }
}
