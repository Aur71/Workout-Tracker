using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class ActiveWorkoutPage : ContentPage, IQueryAttributable
{
    private readonly ActiveWorkoutViewModel _vm;
    private int? _sessionId;

    public ActiveWorkoutPage(ActiveWorkoutViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Content.Opacity = 0;
        if (query.TryGetValue("sessionId", out var idValue) && int.TryParse(idValue?.ToString(), out int id))
        {
            _sessionId = id;
            await _vm.LoadSessionAsync(id);
        }
        await Content.FadeTo(1, 250, Easing.CubicOut);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_sessionId.HasValue && _vm.Exercises.Count > 0)
        {
            Content.Opacity = 0;
            await _vm.LoadSessionAsync(_sessionId.Value);
            await Content.FadeTo(1, 250, Easing.CubicOut);
        }
    }
}
