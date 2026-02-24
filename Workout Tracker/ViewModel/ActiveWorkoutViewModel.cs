using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Workout_Tracker.Messages;
using Workout_Tracker.Model;
using Workout_Tracker.Services;

namespace Workout_Tracker.ViewModel;

public partial class ActiveWorkoutViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private Session? _session;
    private IDispatcherTimer? _timer;
    private DateTime? _workoutStartDateTime;
    private readonly List<(ActiveSetDisplay set, PropertyChangedEventHandler handler)> _setSubscriptions = [];

    private readonly LoadingService _loading;

    public ActiveWorkoutViewModel(DatabaseService db, LoadingService loading)
    {
        _db = db;
        _loading = loading;
    }

    public ObservableCollection<ActiveExerciseDisplay> Exercises { get; } = [];

    [ObservableProperty]
    private string _sessionTitle = string.Empty;

    [ObservableProperty]
    private string _sessionDateDisplay = string.Empty;

    [ObservableProperty]
    private string _elapsedTimeDisplay = "00:00";

    [ObservableProperty]
    private bool _isWorkoutActive;

    [ObservableProperty]
    private string _progressDisplay = "0/0 sets";

    [ObservableProperty]
    private int _totalSetCount;

    [ObservableProperty]
    private int _completedSetCount;

    [ObservableProperty]
    private bool _isReadOnly;

    [ObservableProperty]
    private string _elapsedDurationDisplay = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isVideoOverlayVisible;

    [ObservableProperty]
    private string? _videoOverlayUrl;

    public async Task LoadSessionAsync(int sessionId)
    {
        await _loading.RunAsync(async () =>
        {
        _session = await _db.GetSessionByIdAsync(sessionId);
        if (_session == null) return;

        // Load program name
        if (_session.ProgramId.HasValue)
        {
            var program = await _db.GetProgramByIdAsync(_session.ProgramId.Value);
            SessionTitle = program?.Name ?? "Workout";
        }
        else
        {
            SessionTitle = "Workout";
        }

        SessionDateDisplay = _session.Date.ToString("ddd, MMM d");

        // Unsubscribe old handlers before clearing
        UnsubscribeFromSets();

        // Load exercises and sets
        var isCompleted = _session.IsCompleted;
        var exercises = await _db.GetActiveWorkoutDataAsync(sessionId);
        Exercises.Clear();
        foreach (var ex in exercises)
        {
            ex.IsReadOnly = isCompleted;

            // Subscribe to set completion changes (only for active workouts)
            if (!isCompleted)
            {
                foreach (var set in ex.Sets)
                {
                    PropertyChangedEventHandler handler = (_, args) =>
                    {
                        if (args.PropertyName == nameof(ActiveSetDisplay.Completed))
                        {
                            ex.RefreshCompletedCount();
                            UpdateProgress();
                        }
                    };
                    set.PropertyChanged += handler;
                    _setSubscriptions.Add((set, handler));
                }
            }
            Exercises.Add(ex);
        }

        UpdateProgress();

        // Completed session → read-only mode
        if (_session.IsCompleted)
        {
            IsReadOnly = true;
            IsWorkoutActive = false;

            // Show total elapsed duration
            if (_session.StartTime.HasValue && _session.EndTime.HasValue)
            {
                var duration = _session.EndTime.Value - _session.StartTime.Value;
                if (duration.TotalSeconds < 0)
                    duration += TimeSpan.FromHours(24);
                ElapsedDurationDisplay = duration.TotalHours >= 1
                    ? duration.ToString(@"h\:mm\:ss")
                    : duration.ToString(@"mm\:ss");
            }
        }
        // Resume if workout was already started
        else if (_session.StartTime.HasValue)
        {
            IsWorkoutActive = true;
            // Reconstruct full DateTime from the stored TimeOfDay
            _workoutStartDateTime = DateTime.Today + _session.StartTime.Value;
            // If the reconstructed time is in the future, the workout started yesterday
            if (_workoutStartDateTime > DateTime.Now)
                _workoutStartDateTime = _workoutStartDateTime.Value.AddDays(-1);
            StartTimer();
        }
        }, "Loading...");
    }

    private void UnsubscribeFromSets()
    {
        foreach (var (set, handler) in _setSubscriptions)
            set.PropertyChanged -= handler;
        _setSubscriptions.Clear();
    }

    private void UpdateProgress()
    {
        TotalSetCount = Exercises.Sum(e => e.TotalSetCount);
        CompletedSetCount = Exercises.Sum(e => e.CompletedSetCount);
        ProgressDisplay = $"{CompletedSetCount}/{TotalSetCount} sets";
    }

    [RelayCommand]
    private async Task StartWorkout()
    {
        if (_session == null) return;

        _session.StartTime = DateTime.Now.TimeOfDay;
        _workoutStartDateTime = DateTime.Now;
        await _db.UpdateSessionAsync(_session);

        IsWorkoutActive = true;
        StartTimer();
    }

    private void StartTimer()
    {
        if (_workoutStartDateTime == null) return;

        _timer?.Stop();
        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) => UpdateElapsed();
        _timer.Start();
        UpdateElapsed();
    }

    private void UpdateElapsed()
    {
        if (_workoutStartDateTime == null) return;

        var elapsed = DateTime.Now - _workoutStartDateTime.Value;
        if (elapsed.TotalSeconds < 0) elapsed = TimeSpan.Zero;

        ElapsedTimeDisplay = elapsed.TotalHours >= 1
            ? elapsed.ToString(@"h\:mm\:ss")
            : elapsed.ToString(@"mm\:ss");
    }

    [RelayCommand]
    private void ShowVideo(ActiveExerciseDisplay exercise)
    {
        VideoOverlayUrl = exercise.VideoEmbedUrl;
        IsVideoOverlayVisible = true;
    }

    [RelayCommand]
    private void CloseVideo()
    {
        IsVideoOverlayVisible = false;
        VideoOverlayUrl = "about:blank";
    }

    [RelayCommand]
    private void ToggleExerciseExpand(ActiveExerciseDisplay exercise)
    {
        exercise.IsExpanded = !exercise.IsExpanded;
    }

    [RelayCommand]
    private async Task FinishWorkout()
    {
        if (_session == null || IsBusy) return;
        IsBusy = true;

        try
        {
            await _loading.RunAsync(async () =>
            {
                // Save all sets
                await _db.SaveActiveWorkoutSetsAsync(Exercises.ToList());

                // Mark session completed
                _session.EndTime = DateTime.Now.TimeOfDay;
                _session.IsCompleted = true;
                await _db.UpdateSessionAsync(_session);

                Cleanup();
                WeakReferenceMessenger.Default.Send(new SessionCompletedMessage(_session.Id));
                await Shell.Current.GoToAsync("..");
            }, "Saving...");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelWorkout()
    {
        // Save progress so user can resume
        if (IsWorkoutActive)
            await _db.SaveActiveWorkoutSetsAsync(Exercises.ToList());

        Cleanup();
        await Shell.Current.GoToAsync("..");
    }

    public async Task SaveProgressAsync()
    {
        if (_session != null && IsWorkoutActive)
            await _db.SaveActiveWorkoutSetsAsync(Exercises.ToList());
    }

    public void Cleanup()
    {
        _timer?.Stop();
        _timer = null;
        UnsubscribeFromSets();
    }
}
