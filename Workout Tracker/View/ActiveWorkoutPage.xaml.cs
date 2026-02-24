using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class ActiveWorkoutPage : ContentPage, IQueryAttributable
{
    private readonly ActiveWorkoutViewModel _vm;
    private int? _sessionId;
    private bool _skipNextAppearing;

    public ActiveWorkoutPage(ActiveWorkoutViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.AddLoadingOverlay();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        try
        {
            if (query.TryGetValue("sessionId", out var idValue) && int.TryParse(idValue?.ToString(), out int id))
            {
                _sessionId = id;
                _skipNextAppearing = true;
                await _vm.LoadSessionAsync(id);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_skipNextAppearing)
        {
            _skipNextAppearing = false;
            return;
        }
        // Don't reload during an active workout — would discard unsaved input
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.CloseVideoCommand.Execute(null);
        try
        {
            // Save progress if workout is active so data isn't lost on back gesture
            await _vm.SaveProgressAsync();
        }
        catch { /* best-effort save */ }
        _vm.Cleanup();
    }
}
