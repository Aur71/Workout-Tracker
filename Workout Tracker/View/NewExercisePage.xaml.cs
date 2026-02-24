using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class NewExercisePage : ContentPage, IQueryAttributable
{
    private readonly NewExerciseViewModel _vm;

    public NewExercisePage(NewExerciseViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.AddLoadingOverlay();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        try { await VideoPreview.EvaluateJavaScriptAsync("document.querySelectorAll('video,audio').forEach(m=>m.pause());document.querySelectorAll('iframe').forEach(f=>{f.src=''})"); } catch { }
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        try
        {
            if (query.TryGetValue("edit", out var editValue) && editValue?.ToString() == "true")
            {
                PageTitle.Text = "Edit Exercise";

                if (query.TryGetValue("id", out var idValue) && int.TryParse(idValue?.ToString(), out int id))
                {
                    await _vm.LoadExerciseAsync(id);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
