using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class ExerciseDetailPage : ContentPage, IQueryAttributable
{
    private readonly ExerciseDetailViewModel _vm;
    private int? _exerciseId;
    private bool _skipNextAppearing;

    public ExerciseDetailPage(ExerciseDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.AddLoadingOverlay();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        try
        {
            if (query.TryGetValue("id", out var idValue) && int.TryParse(idValue?.ToString(), out int id))
            {
                _exerciseId = id;
                _skipNextAppearing = true;
                await _vm.LoadExerciseAsync(id);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        try { await VideoPreview.EvaluateJavaScriptAsync("document.querySelectorAll('video,audio').forEach(m=>m.pause());document.querySelectorAll('iframe').forEach(f=>{f.src=''})"); } catch { }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            if (_skipNextAppearing)
            {
                _skipNextAppearing = false;
                return;
            }
            // Reload after returning from edit to show updated data
            if (_exerciseId.HasValue)
                await _vm.LoadExerciseAsync(_exerciseId.Value);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
