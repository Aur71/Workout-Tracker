using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Services;

namespace Workout_Tracker.ViewModel;

public partial class SettingsViewModel : ObservableObject
{
    private readonly DataTransferService _transfer;

    public SettingsViewModel(DataTransferService transfer)
    {
        _transfer = transfer;
    }

    [ObservableProperty]
    private bool _isExporting;

    [ObservableProperty]
    private bool _isImporting;

    [RelayCommand]
    private async Task ExportData()
    {
        if (IsExporting) return;

        try
        {
            IsExporting = true;
            var filePath = await _transfer.ExportAsync();

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Export Workout Data",
                File = new ShareFile(filePath)
            });
        }
        catch (Exception ex)
        {
            var page = Shell.Current.CurrentPage;
            await page.DisplayAlert("Export Failed", ex.Message, "OK");
        }
        finally
        {
            IsExporting = false;
        }
    }

    [RelayCommand]
    private async Task ImportData()
    {
        if (IsImporting) return;

        try
        {
            var fileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "application/json" } },
                { DevicePlatform.iOS, new[] { "public.json" } },
                { DevicePlatform.WinUI, new[] { ".json" } },
                { DevicePlatform.macOS, new[] { "public.json" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select Backup File",
                FileTypes = fileTypes
            });

            if (result == null) return;

            var page = Shell.Current.CurrentPage;
            bool confirm = await page.DisplayAlert(
                "Import Data",
                "This will replace ALL existing data with the backup. This cannot be undone. Continue?",
                "Import",
                "Cancel");

            if (!confirm) return;

            IsImporting = true;

            using var stream = await result.OpenReadAsync();
            await _transfer.ImportAsync(stream);

            await page.DisplayAlert("Import Complete", "Your data has been restored successfully.", "OK");
        }
        catch (InvalidOperationException ex)
        {
            var page = Shell.Current.CurrentPage;
            await page.DisplayAlert("Import Failed", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            var page = Shell.Current.CurrentPage;
            await page.DisplayAlert("Import Failed", $"An unexpected error occurred: {ex.Message}", "OK");
        }
        finally
        {
            IsImporting = false;
        }
    }
}
