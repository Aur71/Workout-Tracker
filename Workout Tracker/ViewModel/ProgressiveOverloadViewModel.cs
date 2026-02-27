using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Model;
using Workout_Tracker.Services;

namespace Workout_Tracker.ViewModel;

public partial class ProgressiveOverloadViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly LoadingService _loading;
    private int _programId;

    private List<(Session session, List<(SessionExercise se, List<Set> sets)> exercises, List<string> tags)> _baseData = [];
    private Dictionary<int, string> _exerciseNames = [];

    public ProgressiveOverloadViewModel(DatabaseService db, LoadingService loading)
    {
        _db = db;
        _loading = loading;
    }

    [ObservableProperty]
    private int _cycleCount = 4;

    public ObservableCollection<SessionOverloadConfig> SessionConfigs { get; } = [];

    public ObservableCollection<OverloadPreviewCycle> Preview { get; } = [];

    [ObservableProperty]
    private bool _hasPreview;

    [ObservableProperty]
    private int _totalSessionsToGenerate;

    [ObservableProperty]
    private int _baseSessionCount;

    public bool HasBaseSessions => BaseSessionCount > 0;

    partial void OnCycleCountChanged(int value)
    {
        TotalSessionsToGenerate = value * BaseSessionCount;
        HasPreview = false;
        Preview.Clear();
    }

    public async Task LoadAsync(int programId)
    {
        _programId = programId;
        await _loading.RunAsync(async () =>
        {
            _baseData = await _db.GetBaseSessionDataAsync(programId);
            BaseSessionCount = _baseData.Count;
            TotalSessionsToGenerate = CycleCount * BaseSessionCount;
            OnPropertyChanged(nameof(HasBaseSessions));

            var exercises = await _db.GetAllExercisesAsync();
            _exerciseNames = exercises.ToDictionary(e => e.Id, e => e.Name);

            SessionConfigs.Clear();
            for (int i = 0; i < _baseData.Count; i++)
            {
                var (session, sessionExercises, _) = _baseData[i];
                var config = new SessionOverloadConfig
                {
                    SessionIndex = i,
                    SessionLabel = $"Session {i + 1} — {session.Date:MMM d}"
                };

                foreach (var (se, sets) in sessionExercises)
                {
                    var workingSets = sets.Where(s => !s.IsWarmup).ToList();
                    var firstSet = workingSets.FirstOrDefault();

                    config.Exercises.Add(new ExerciseOverloadConfig
                    {
                        SessionIndex = i,
                        ExerciseId = se.ExerciseId,
                        ExerciseName = _exerciseNames.GetValueOrDefault(se.ExerciseId, "Unknown"),
                        SessionLabel = config.SessionLabel,
                        BaseWeight = firstSet?.PlannedWeight,
                        BaseSetCount = workingSets.Count,
                        BaseRepMin = firstSet?.RepMin,
                        BaseRepMax = firstSet?.RepMax,
                        BaseRpe = firstSet?.PlannedRpe
                    });
                }

                config.InitializeChips();
                SessionConfigs.Add(config);
            }
        }, "Loading...");
    }

    [RelayCommand]
    private void GeneratePreview()
    {
        Preview.Clear();

        if (_baseData.Count == 0 || CycleCount <= 0)
        {
            HasPreview = false;
            return;
        }

        for (int cycle = 1; cycle <= CycleCount; cycle++)
        {
            var cyclePreview = new OverloadPreviewCycle
            {
                CycleNumber = cycle,
                SessionCount = _baseData.Count,
                WeekLabel = $"Cycle {cycle}"
            };

            for (int si = 0; si < _baseData.Count; si++)
            {
                var (_, sessionExercises, _) = _baseData[si];
                var sessionConfig = SessionConfigs.FirstOrDefault(c => c.SessionIndex == si);
                if (sessionConfig == null) continue;

                var method = sessionConfig.SelectedMethod;
                double.TryParse(sessionConfig.RpeIncrementText, out var rpeIncrement);
                int.TryParse(sessionConfig.StepCyclesText, out var stepCyc);
                int.TryParse(sessionConfig.DoubleProgressionCyclesText, out var doubleCyc);

                foreach (var (se, sets) in sessionExercises)
                {
                    var workingSets = sets.Where(s => !s.IsWarmup).ToList();
                    if (workingSets.Count == 0) continue;

                    var config = sessionConfig.Exercises.FirstOrDefault(
                        c => c.ExerciseId == se.ExerciseId);
                    if (config == null) continue;

                    double.TryParse(config.IncrementText, out var increment);
                    int.TryParse(config.SetsToAddText, out var setsToAdd);
                    int.TryParse(config.EveryNCyclesText, out var everyN);
                    if (everyN < 1) everyN = 1;

                    var previewEx = BuildPreviewExercise(
                        method, config.ExerciseName, config.SessionLabel,
                        workingSets, cycle, increment, setsToAdd, everyN,
                        rpeIncrement, stepCyc, doubleCyc,
                        sessionConfig.IncludeBaseCycle);

                    cyclePreview.Exercises.Add(previewEx);
                }
            }

            Preview.Add(cyclePreview);
        }

        TotalSessionsToGenerate = CycleCount * _baseData.Count;
        HasPreview = true;
    }

    private static OverloadPreviewExercise BuildPreviewExercise(
        OverloadMethod method,
        string exerciseName,
        string sessionLabel,
        List<Set> workingSets,
        int cycleIndex,
        double weightIncrement,
        int setsToAdd,
        int everyNCycles,
        double rpeIncrement,
        int stepCyc,
        int doubleCyc,
        bool includeBaseCycle)
    {
        var first = workingSets.First();
        int setCount = workingSets.Count;
        int? repMin = first.RepMin;
        int? repMax = first.RepMax;
        double? weight = first.PlannedWeight;
        double? rpe = first.PlannedRpe;
        string changeNote = "";

        switch (method)
        {
            case OverloadMethod.Linear:
                if (weight.HasValue)
                {
                    weight = weight.Value + (weightIncrement * cycleIndex);
                    changeNote = $"+{weightIncrement * cycleIndex}kg";
                }
                break;

            case OverloadMethod.Double:
                int cyclesBeforeJump = Math.Max(1, doubleCyc);
                int doubleOffset = includeBaseCycle ? cycleIndex : cycleIndex - 1;
                int weightPhase = doubleOffset / cyclesBeforeJump;
                int posInPhase = doubleOffset % cyclesBeforeJump;

                if (weight.HasValue)
                    weight = weight.Value + (weightIncrement * weightPhase);

                if (repMin.HasValue && repMax.HasValue && cyclesBeforeJump > 1)
                {
                    int range = repMax.Value - repMin.Value;
                    double repStep = (double)range / (cyclesBeforeJump - 1);
                    repMin = first.RepMin!.Value + (int)Math.Round(repStep * posInPhase);
                    if (repMin > repMax) repMin = repMax;
                }

                if (weightPhase > 0)
                    changeNote = $"+{weightIncrement * weightPhase}kg";
                else if (repMin != first.RepMin)
                    changeNote = $"reps: {repMin}";
                break;

            case OverloadMethod.Volume:
                int volumeOffset = includeBaseCycle ? cycleIndex + 1 : cycleIndex;
                int volumeSteps = volumeOffset / Math.Max(1, everyNCycles);
                int extraSets = setsToAdd * volumeSteps;
                setCount += extraSets;
                if (extraSets > 0)
                    changeNote = $"+{extraSets} sets";
                else
                    changeNote = "hold";
                break;

            case OverloadMethod.Rpe:
                if (rpe.HasValue)
                {
                    rpe = Math.Min(10, rpe.Value + (rpeIncrement * cycleIndex));
                    changeNote = $"RPE {rpe}";
                }
                break;

            case OverloadMethod.StepLoading:
                int stepOffset = includeBaseCycle ? cycleIndex : cycleIndex - 1;
                int step = stepOffset / Math.Max(1, stepCyc);
                if (weight.HasValue && step > 0)
                {
                    weight = weight.Value + (weightIncrement * step);
                    changeNote = $"+{weightIncrement * step}kg";
                }
                else if (step == 0)
                    changeNote = "hold";
                break;
        }

        string repsDisplay = "";
        if (repMin.HasValue && repMax.HasValue && repMin != repMax)
            repsDisplay = $"{repMin}-{repMax}";
        else if (repMin.HasValue)
            repsDisplay = repMin.Value.ToString();
        else if (repMax.HasValue)
            repsDisplay = repMax.Value.ToString();

        return new OverloadPreviewExercise
        {
            ExerciseName = exerciseName,
            SessionLabel = sessionLabel,
            SetsRepsDisplay = string.IsNullOrEmpty(repsDisplay) ? $"{setCount} sets" : $"{setCount}x{repsDisplay}",
            WeightDisplay = weight.HasValue ? $"{weight}kg" : "",
            RpeDisplay = rpe.HasValue ? $"RPE {rpe}" : "",
            ChangeNote = changeNote
        };
    }

    [RelayCommand]
    private async Task Generate()
    {
        if (_baseData.Count == 0 || CycleCount <= 0) return;

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Generate Sessions",
            $"This will create {CycleCount * _baseData.Count} new sessions with progressive overload. Continue?",
            "Generate", "Cancel");

        if (!confirm) return;

        await _loading.RunAsync(async () =>
        {
            var sessionMethods = new Dictionary<int, OverloadMethod>();
            var rpeIncrements = new Dictionary<int, double>();
            var stepCyclesMap = new Dictionary<int, int>();
            var doubleCyclesMap = new Dictionary<int, int>();
            var includeBaseCycleMap = new Dictionary<int, bool>();
            var weightIncrements = new Dictionary<(int, int), double>();
            var volumeConfigs = new Dictionary<(int, int), (int setsToAdd, int everyNCycles)>();

            foreach (var sc in SessionConfigs)
            {
                sessionMethods[sc.SessionIndex] = sc.SelectedMethod;

                double.TryParse(sc.RpeIncrementText, out var rpeInc);
                rpeIncrements[sc.SessionIndex] = rpeInc;

                int.TryParse(sc.StepCyclesText, out var stepCyc);
                stepCyclesMap[sc.SessionIndex] = stepCyc;

                int.TryParse(sc.DoubleProgressionCyclesText, out var doubleCyc);
                doubleCyclesMap[sc.SessionIndex] = doubleCyc;

                includeBaseCycleMap[sc.SessionIndex] = sc.IncludeBaseCycle;

                foreach (var ex in sc.Exercises)
                {
                    var key = (ex.SessionIndex, ex.ExerciseId);
                    double.TryParse(ex.IncrementText, out var inc);
                    weightIncrements[key] = inc;

                    int.TryParse(ex.SetsToAddText, out var sta);
                    int.TryParse(ex.EveryNCyclesText, out var enc);
                    if (enc < 1) enc = 1;
                    volumeConfigs[key] = (sta, enc);
                }
            }

            await _db.GenerateProgressiveOverloadAsync(
                _programId, CycleCount, sessionMethods,
                weightIncrements, volumeConfigs,
                rpeIncrements, stepCyclesMap, doubleCyclesMap,
                includeBaseCycleMap);

            await Shell.Current.GoToAsync("..");
        }, "Generating...");
    }

    [RelayCommand]
    private async Task Back()
    {
        await Shell.Current.GoToAsync("..");
    }
}
