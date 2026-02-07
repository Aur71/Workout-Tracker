using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class ExerciseDetailPage : ContentPage, IQueryAttributable
{
    private readonly ExerciseDetailViewModel _vm;

    public ExerciseDetailPage(ExerciseDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var idValue) && int.TryParse(idValue?.ToString(), out int id))
        {
            await _vm.LoadExerciseAsync(id);
        }
    }
}
