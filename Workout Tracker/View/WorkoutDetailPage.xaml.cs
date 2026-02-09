using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class WorkoutDetailPage : ContentPage, IQueryAttributable
{
    private readonly WorkoutDetailViewModel _vm;

    public WorkoutDetailPage(WorkoutDetailViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Content.Opacity = 0;
        if (query.TryGetValue("id", out var idValue) && int.TryParse(idValue?.ToString(), out var id))
        {
            await _vm.LoadWorkoutAsync(id);
        }
        await Content.FadeTo(1, 250, Easing.CubicOut);
    }
}
