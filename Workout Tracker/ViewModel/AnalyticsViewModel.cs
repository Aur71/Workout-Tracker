using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Workout_Tracker.Drawables;
using Workout_Tracker.Model;
using Workout_Tracker.Services;

namespace Workout_Tracker.ViewModel;

public partial class AnalyticsViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private bool _exercisesLoaded;

    private readonly LoadingService _loading;

    public AnalyticsViewModel(DatabaseService db, LoadingService loading)
    {
        _db = db;
        _loading = loading;
    }

    // ── Time range ──

    [ObservableProperty]
    private DateTime _startDate = DateTime.Today.AddMonths(-3);

    [ObservableProperty]
    private DateTime _endDate = DateTime.Today;

    [ObservableProperty]
    private bool _isLoading;

    // ── Summary ──

    [ObservableProperty]
    private string _totalSessionsDisplay = "0";

    [ObservableProperty]
    private string _totalSetsDisplay = "0";

    [ObservableProperty]
    private string _totalVolumeDisplay = "0";

    [ObservableProperty]
    private string _avgDurationDisplay = "0";

    // ── Chart Drawables ──

    [ObservableProperty]
    private IDrawable? _muscleVolumeDrawable;

    [ObservableProperty]
    private bool _hasMuscleVolumeData;

    public ObservableCollection<ExercisePickerItem> AvailableExercises { get; } = [];

    [ObservableProperty]
    private ExercisePickerItem? _selectedExercise;

    [ObservableProperty]
    private IDrawable? _progressionDrawable;

    [ObservableProperty]
    private bool _hasProgressionData;

    [ObservableProperty]
    private IDrawable? _adherenceDrawable;

    [ObservableProperty]
    private bool _hasAdherenceData;

    [ObservableProperty]
    private string _adherencePercentDisplay = "";

    [ObservableProperty]
    private IDrawable? _bodyMetricDrawable;

    [ObservableProperty]
    private bool _hasBodyMetricData;

    // ── Theme Colors ──

    private static Color PrimaryColor => Color.FromArgb("#00D9A5");
    private static Color SecondaryColor => Color.FromArgb("#FF6B5B");
    private static Color TertiaryColor => Color.FromArgb("#4A6CF7");

    private static Color TextColor =>
        Application.Current?.RequestedTheme == AppTheme.Dark
            ? Colors.White
            : Color.FromArgb("#1E1E1E");

    private static Color GridColor =>
        Application.Current?.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#3C3C3C")
            : Color.FromArgb("#DCDCDC");

    // ── Commands ──

    partial void OnStartDateChanged(DateTime value) => _ = LoadAsync();

    partial void OnEndDateChanged(DateTime value) => _ = LoadAsync();

    partial void OnSelectedExerciseChanged(ExercisePickerItem? value)
    {
        if (value != null)
            _ = LoadProgressionChartAsync();
    }

    // ── Data Loading ──

    public async Task LoadAsync()
    {
        await _loading.RunAsync(async () =>
        {
        IsLoading = true;

        try
        {
            var (from, to) = GetDateRange();

            if (!_exercisesLoaded)
            {
                var exercises = await _db.GetExercisesWithCompletedSetsAsync();
                AvailableExercises.Clear();
                foreach (var e in exercises)
                    AvailableExercises.Add(e);
                _exercisesLoaded = true;

                if (AvailableExercises.Count > 0 && SelectedExercise == null)
                    SelectedExercise = AvailableExercises[0];
            }

            var summaryTask = _db.GetAnalyticsSummaryAsync(from, to);
            var muscleTask = _db.GetMuscleVolumeAsync(from, to);
            var adherenceTask = _db.GetSessionAdherenceAsync(from, to);
            var bodyMetricTask = _db.GetBodyMetricTrendAsync(from, to);

            await Task.WhenAll(summaryTask, muscleTask, adherenceTask, bodyMetricTask);

            var summary = summaryTask.Result;
            var muscleData = muscleTask.Result;
            var adherenceData = adherenceTask.Result;
            var bodyMetricData = bodyMetricTask.Result;

            // Summary
            TotalSessionsDisplay = summary.TotalSessions.ToString();
            TotalSetsDisplay = summary.TotalSetsCompleted.ToString();
            TotalVolumeDisplay = summary.TotalVolume >= 1000
                ? $"{summary.TotalVolume / 1000.0:0.#}k"
                : summary.TotalVolume.ToString("0");
            AvgDurationDisplay = summary.AvgSessionDurationMinutes > 0
                ? $"{summary.AvgSessionDurationMinutes:0}"
                : "\u2014";

            BuildMuscleVolumeChart(muscleData);
            BuildAdherenceChart(adherenceData);
            BuildBodyMetricChart(bodyMetricData);

            if (SelectedExercise != null)
                await LoadProgressionChartAsync();
        }
        finally
        {
            IsLoading = false;
        }
        }, "Loading...");
    }

    private (DateTime from, DateTime to) GetDateRange()
    {
        return (StartDate, EndDate);
    }

    // ── Chart Builders ──

    private void BuildMuscleVolumeChart(List<MuscleVolumeData> data)
    {
        var top10 = data.Take(10).ToList();
        HasMuscleVolumeData = top10.Count > 0;

        if (!HasMuscleVolumeData)
        {
            MuscleVolumeDrawable = null;
            return;
        }

        var entries = top10.Select(m => new BarEntry(m.MuscleName, m.TotalSets)).ToList();
        MuscleVolumeDrawable = new HorizontalBarChartDrawable(entries, PrimaryColor, TextColor, GridColor);
    }

    private async Task LoadProgressionChartAsync()
    {
        if (SelectedExercise == null) return;

        var (from, to) = GetDateRange();
        var data = await _db.GetExerciseProgressionAsync(SelectedExercise.Id, from, to);

        HasProgressionData = data.Count > 0;

        if (!HasProgressionData)
        {
            ProgressionDrawable = null;
            return;
        }

        var e1rmLine = new ChartLine("Est. 1RM (kg)", PrimaryColor,
            data.Where(d => d.EstimatedOneRepMax > 0)
                .Select(d => new ChartDataPoint(d.Date, d.EstimatedOneRepMax)).ToList(), 0);

        var bestWeightLine = new ChartLine("Best Weight (kg)", SecondaryColor,
            data.Select(d => new ChartDataPoint(d.Date, d.BestWeight)).ToList(), 0);

        var volumeLine = new ChartLine("Volume (kg)", TertiaryColor,
            data.Select(d => new ChartDataPoint(d.Date, d.TotalVolume)).ToList(), 1);

        var series = new List<ChartLine>();
        if (e1rmLine.Points.Count > 0) series.Add(e1rmLine);
        series.Add(bestWeightLine);
        series.Add(volumeLine);

        ProgressionDrawable = new LineChartDrawable(
            series, TextColor, GridColor, "Weight", "Volume");
    }

    private void BuildAdherenceChart(List<SessionAdherenceData> data)
    {
        HasAdherenceData = data.Count > 0;

        if (!HasAdherenceData)
        {
            AdherenceDrawable = null;
            AdherencePercentDisplay = "";
            return;
        }

        var totalPlanned = data.Sum(d => d.PlannedSets);
        var totalCompleted = data.Sum(d => d.CompletedSets);
        var pct = totalPlanned > 0 ? (double)totalCompleted / totalPlanned * 100 : 0;
        AdherencePercentDisplay = $"{pct:0}%";

        var groups = data.Select(d => new ColumnGroup(
            d.Date.ToString("MMM d"), d.PlannedSets, d.CompletedSets)).ToList();

        AdherenceDrawable = new GroupedColumnChartDrawable(
            groups, GridColor, PrimaryColor, "Planned", "Completed", TextColor, GridColor);
    }

    private void BuildBodyMetricChart(List<BodyMetricPoint> data)
    {
        HasBodyMetricData = data.Count > 0;

        if (!HasBodyMetricData)
        {
            BodyMetricDrawable = null;
            return;
        }

        var series = new List<ChartLine>();
        bool hasWeight = data.Any(d => d.Bodyweight.HasValue);
        bool hasBf = data.Any(d => d.BodyFat.HasValue);

        if (hasWeight)
        {
            series.Add(new ChartLine("Bodyweight (kg)", SecondaryColor,
                data.Where(d => d.Bodyweight.HasValue)
                    .Select(d => new ChartDataPoint(d.Date, d.Bodyweight!.Value)).ToList(), 0));
        }

        if (hasBf)
        {
            series.Add(new ChartLine("Body Fat %", TertiaryColor,
                data.Where(d => d.BodyFat.HasValue)
                    .Select(d => new ChartDataPoint(d.Date, d.BodyFat!.Value)).ToList(),
                hasWeight ? 1 : 0));
        }

        if (series.Count == 0)
        {
            HasBodyMetricData = false;
            BodyMetricDrawable = null;
            return;
        }

        BodyMetricDrawable = new LineChartDrawable(series, TextColor, GridColor, "Weight", hasBf ? "BF %" : null);
    }
}
