using Workout_Tracker.Extensions;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker.View;

public partial class NewSessionPage : ContentPage, IQueryAttributable
{
    private readonly NewSessionViewModel _vm;

    public NewSessionPage(NewSessionViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        this.AddLoadingOverlay();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        try
        {
            if (query.TryGetValue("edit", out var editVal) && editVal?.ToString() == "true"
                && query.TryGetValue("id", out var idVal) && int.TryParse(idVal?.ToString(), out int id))
            {
                PageTitle.Text = "Edit Session";
                await _vm.LoadSessionAsync(id);
            }
            else if (query.TryGetValue("duplicate", out var dupVal) && dupVal?.ToString() == "true"
                && query.TryGetValue("id", out var dupIdVal) && int.TryParse(dupIdVal?.ToString(), out int dupId))
            {
                await _vm.DuplicateSessionAsync(dupId);
            }
            else if (query.TryGetValue("programId", out var pidVal) && int.TryParse(pidVal?.ToString(), out int pid))
            {
                await _vm.InitAsync(pid);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
