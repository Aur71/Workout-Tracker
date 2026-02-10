using CommunityToolkit.Mvvm.ComponentModel;

namespace Workout_Tracker.Services;

public partial class LoadingService : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _loadingMessage = "Loading...";

    public void Show(string message = "Loading...")
    {
        LoadingMessage = message;
        IsLoading = true;
    }

    public void Hide()
    {
        IsLoading = false;
    }

    public async Task RunAsync(Func<Task> action, string message = "Loading...")
    {
        Show(message);
        var minDelay = Task.Delay(200);

        try
        {
            await action();
        }
        finally
        {
            await minDelay;
            Hide();
        }
    }
}
