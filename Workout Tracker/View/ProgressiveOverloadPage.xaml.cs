using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class ProgressiveOverloadPage : ContentPage, IQueryAttributable
{
    private readonly ProgressiveOverloadViewModel _vm;

    public ProgressiveOverloadPage(ProgressiveOverloadViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.AddLoadingOverlay();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        try
        {
            if (query.TryGetValue("programId", out var idValue) && int.TryParse(idValue?.ToString(), out int programId))
                await _vm.LoadAsync(programId);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void OnDecrementCycles(object? sender, TappedEventArgs e)
    {
        if (_vm.CycleCount > 1)
            _vm.CycleCount--;
    }

    private void OnIncrementCycles(object? sender, TappedEventArgs e)
    {
        if (_vm.CycleCount < 20)
            _vm.CycleCount++;
    }
}
