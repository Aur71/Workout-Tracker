using Workout_Tracker.Model;
using Program = Workout_Tracker.Model.Program;

namespace Workout_Tracker.Services;

public class DatabaseService
{
    public async Task<List<Muscle>> GetAllMusclesAsync()
    {
        return await Task.Run(async () =>
            await AppDatabase.Database.Table<Muscle>().ToListAsync());
    }

    public async Task<int> SaveExerciseAsync(Exercise exercise)
    {
        return await Task.Run(async () =>
        {
            await AppDatabase.Database.InsertAsync(exercise);
            return exercise.Id;
        });
    }

    public async Task SaveExerciseMusclesAsync(int exerciseId, List<MuscleSelection> muscles)
    {
        var rows = muscles.Select(m => new ExerciseMuscle
        {
            ExerciseId = exerciseId,
            MuscleId = m.Muscle.Id,
            Type = m.Role
        }).ToList();

        await Task.Run(async () =>
            await AppDatabase.Database.InsertAllAsync(rows));
    }

    public async Task<List<ExerciseDisplay>> GetAllExercisesAsync()
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var exercises = await db.Table<Exercise>().ToListAsync();
            var exerciseMuscles = await db.Table<ExerciseMuscle>().ToListAsync();
            var muscles = await db.Table<Muscle>().ToListAsync();

            var muscleDict = muscles.ToDictionary(m => m.Id, m => m.Name);

            return exercises.Select(e =>
            {
                var eMuscles = exerciseMuscles
                    .Where(em => em.ExerciseId == e.Id)
                    .Select(em => new MuscleDisplay
                    {
                        Name = muscleDict.GetValueOrDefault(em.MuscleId, "Unknown"),
                        Role = em.Type ?? "primary"
                    })
                    .ToList();

                return new ExerciseDisplay
                {
                    Id = e.Id,
                    Name = e.Name,
                    ExerciseType = e.ExerciseType,
                    Equipment = e.Equipment,
                    IsTimeBased = e.IsTimeBased,
                    Description = e.Description,
                    Instructions = e.Instructions,
                    Notes = e.Notes,
                    PrimaryMuscle = eMuscles.FirstOrDefault(m => m.Role == "primary")?.Name,
                    Muscles = eMuscles
                };
            }).ToList();
        });
    }

    public async Task<ExerciseDisplay?> GetExerciseByIdAsync(int id)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var exercise = await db.Table<Exercise>().FirstOrDefaultAsync(e => e.Id == id);
            if (exercise == null) return null;

            var exerciseMuscles = await db.Table<ExerciseMuscle>()
                .Where(em => em.ExerciseId == id).ToListAsync();
            var muscles = await db.Table<Muscle>().ToListAsync();
            var muscleDict = muscles.ToDictionary(m => m.Id, m => m.Name);

            var eMuscles = exerciseMuscles.Select(em => new MuscleDisplay
            {
                Name = muscleDict.GetValueOrDefault(em.MuscleId, "Unknown"),
                Role = em.Type ?? "primary"
            }).ToList();

            return new ExerciseDisplay
            {
                Id = exercise.Id,
                Name = exercise.Name,
                ExerciseType = exercise.ExerciseType,
                Equipment = exercise.Equipment,
                IsTimeBased = exercise.IsTimeBased,
                Description = exercise.Description,
                Instructions = exercise.Instructions,
                Notes = exercise.Notes,
                PrimaryMuscle = eMuscles.FirstOrDefault(m => m.Role == "primary")?.Name,
                Muscles = eMuscles
            };
        });
    }

    public async Task UpdateExerciseAsync(Exercise exercise)
    {
        await Task.Run(async () =>
            await AppDatabase.Database.UpdateAsync(exercise));
    }

    public async Task DeleteExerciseMusclesAsync(int exerciseId)
    {
        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var rows = await db.Table<ExerciseMuscle>()
                .Where(em => em.ExerciseId == exerciseId).ToListAsync();
            foreach (var row in rows)
                await db.DeleteAsync(row);
        });
    }

    public async Task DeleteExerciseAsync(int id)
    {
        await DeleteExerciseFromSessionsAsync(id);

        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            await db.Table<ExerciseMuscle>().DeleteAsync(em => em.ExerciseId == id);
            await db.DeleteAsync(new Exercise { Id = id });
        });
    }

    // ── Program methods ──

    public async Task<int> SaveProgramAsync(Program program)
    {
        return await Task.Run(async () =>
        {
            await AppDatabase.Database.InsertAsync(program);
            return program.Id;
        });
    }

    public async Task<Program?> GetProgramByIdAsync(int id)
    {
        return await Task.Run(async () =>
            await AppDatabase.Database.Table<Program>().FirstOrDefaultAsync(p => p.Id == id));
    }

    public async Task<List<Program>> GetAllProgramsAsync()
    {
        return await Task.Run(async () =>
            await AppDatabase.Database.Table<Program>().ToListAsync());
    }

    public async Task UpdateProgramAsync(Program program)
    {
        await Task.Run(async () =>
            await AppDatabase.Database.UpdateAsync(program));
    }

    public async Task DeleteProgramAsync(int id)
    {
        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var sessions = await db.Table<Session>()
                .Where(s => s.ProgramId == id).ToListAsync();
            foreach (var session in sessions)
            {
                var sessionExercises = await db.Table<SessionExercise>()
                    .Where(se => se.SessionId == session.Id).ToListAsync();
                foreach (var se in sessionExercises)
                    await db.Table<Set>().DeleteAsync(s => s.SessionExerciseId == se.Id);
                await db.Table<SessionExercise>().DeleteAsync(se => se.SessionId == session.Id);
                await db.DeleteAsync(session);
            }
            await db.DeleteAsync(new Program { Id = id });
        });
    }

    // ── Session methods ──

    public async Task<int> SaveSessionAsync(Session session)
    {
        return await Task.Run(async () =>
        {
            await AppDatabase.Database.InsertAsync(session);
            return session.Id;
        });
    }

    public async Task UpdateSessionAsync(Session session)
    {
        await Task.Run(async () =>
            await AppDatabase.Database.UpdateAsync(session));
    }

    public async Task DeleteSessionAsync(int sessionId)
    {
        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            // Delete sets for all session exercises
            var sessionExercises = await db.Table<SessionExercise>()
                .Where(se => se.SessionId == sessionId).ToListAsync();
            foreach (var se in sessionExercises)
                await db.Table<Set>().DeleteAsync(s => s.SessionExerciseId == se.Id);
            // Delete session exercises
            await db.Table<SessionExercise>().DeleteAsync(se => se.SessionId == sessionId);
            // Delete session
            await db.DeleteAsync(new Session { Id = sessionId });
        });
    }

    public async Task<List<SessionDisplay>> GetSessionsForProgramAsync(int programId)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var sessions = await db.Table<Session>()
                .Where(s => s.ProgramId == programId)
                .OrderBy(s => s.Date)
                .ToListAsync();

            var sessionExercises = await db.Table<SessionExercise>().ToListAsync();
            var sets = await db.Table<Set>().ToListAsync();

            return sessions.Select(s =>
            {
                var seIds = sessionExercises
                    .Where(se => se.SessionId == s.Id)
                    .Select(se => se.Id)
                    .ToList();

                return new SessionDisplay
                {
                    Id = s.Id,
                    Date = s.Date,
                    Notes = s.Notes,
                    ExerciseCount = seIds.Count,
                    SetCount = sets.Count(st => seIds.Contains(st.SessionExerciseId)),
                    IsCompleted = s.IsCompleted
                };
            }).ToList();
        });
    }

    public async Task<(int total, int completed)> GetSessionCountsForProgramAsync(int programId)
    {
        return await Task.Run(async () =>
        {
            var sessions = await AppDatabase.Database.Table<Session>()
                .Where(s => s.ProgramId == programId)
                .ToListAsync();
            return (sessions.Count, sessions.Count(s => s.IsCompleted));
        });
    }

    public async Task<DateTime?> GetLastSessionDateForProgramAsync(int programId)
    {
        return await Task.Run(async () =>
        {
            var sessions = await AppDatabase.Database.Table<Session>()
                .Where(s => s.ProgramId == programId)
                .OrderByDescending(s => s.Date)
                .ToListAsync();

            return sessions.FirstOrDefault()?.Date;
        });
    }

    public async Task<Session?> GetSessionByIdAsync(int sessionId)
    {
        return await Task.Run(async () =>
            await AppDatabase.Database.Table<Session>().FirstOrDefaultAsync(s => s.Id == sessionId));
    }

    public async Task<List<SessionExerciseDisplay>> GetSessionExercisesWithSetsAsync(int sessionId)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var sessionExercises = await db.Table<SessionExercise>()
                .Where(se => se.SessionId == sessionId)
                .OrderBy(se => se.Order)
                .ToListAsync();

            var exercises = await db.Table<Exercise>().ToListAsync();
            var exerciseMuscles = await db.Table<ExerciseMuscle>().ToListAsync();
            var muscles = await db.Table<Muscle>().ToListAsync();
            var sets = await db.Table<Set>().ToListAsync();

            var muscleDict = muscles.ToDictionary(m => m.Id, m => m.Name);
            var exerciseDict = exercises.ToDictionary(e => e.Id);

            return sessionExercises.Select(se =>
            {
                exerciseDict.TryGetValue(se.ExerciseId, out var ex);
                var primaryMuscle = ex != null
                    ? exerciseMuscles
                        .Where(em => em.ExerciseId == ex.Id && em.Type == "primary")
                        .Select(em => muscleDict.GetValueOrDefault(em.MuscleId))
                        .FirstOrDefault()
                    : null;

                var display = new SessionExerciseDisplay
                {
                    ExerciseId = se.ExerciseId,
                    ExerciseName = ex?.Name ?? "Unknown",
                    ExerciseType = ex?.ExerciseType,
                    PrimaryMuscle = primaryMuscle,
                    IsTimeBased = ex?.IsTimeBased ?? false,
                    Order = se.Order,
                    Notes = se.Notes,
                    RestSecondsText = (se.RestSeconds ?? 120).ToString()
                };

                var exerciseSets = sets
                    .Where(s => s.SessionExerciseId == se.Id)
                    .OrderBy(s => s.SetNumber)
                    .Select(s => new SetDisplay
                    {
                        SetNumber = s.SetNumber,
                        RepMinText = s.RepMin?.ToString() ?? "",
                        RepMaxText = s.RepMax?.ToString() ?? "",
                        DurationMinText = s.DurationMin?.ToString() ?? "",
                        DurationMaxText = s.DurationMax?.ToString() ?? "",
                        IsWarmup = s.IsWarmup
                    });

                foreach (var set in exerciseSets)
                    display.Sets.Add(set);

                return display;
            }).ToList();
        });
    }

    public async Task SaveSessionExercisesWithSetsAsync(int sessionId, List<SessionExerciseDisplay> exercises)
    {
        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            foreach (var ex in exercises)
            {
                int.TryParse(ex.RestSecondsText, out var restSec);
                var sessionExercise = new SessionExercise
                {
                    SessionId = sessionId,
                    ExerciseId = ex.ExerciseId,
                    Order = ex.Order,
                    Notes = ex.Notes,
                    RestSeconds = restSec > 0 ? restSec : 120
                };
                await db.InsertAsync(sessionExercise);

                foreach (var set in ex.Sets)
                {
                    int.TryParse(set.RepMinText, out var repMin);
                    int.TryParse(set.RepMaxText, out var repMax);
                    int.TryParse(set.DurationMinText, out var durMin);
                    int.TryParse(set.DurationMaxText, out var durMax);

                    var dbSet = new Set
                    {
                        SessionExerciseId = sessionExercise.Id,
                        SetNumber = set.SetNumber,
                        IsWarmup = set.IsWarmup,
                        RepMin = repMin > 0 ? repMin : null,
                        RepMax = repMax > 0 ? repMax : null,
                        DurationMin = durMin > 0 ? durMin : null,
                        DurationMax = durMax > 0 ? durMax : null,
                        Completed = false
                    };
                    await db.InsertAsync(dbSet);
                }
            }
        });
    }

    public async Task DeleteSessionExercisesAndSetsAsync(int sessionId)
    {
        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var sessionExercises = await db.Table<SessionExercise>()
                .Where(se => se.SessionId == sessionId).ToListAsync();
            foreach (var se in sessionExercises)
                await db.Table<Set>().DeleteAsync(s => s.SessionExerciseId == se.Id);
            await db.Table<SessionExercise>().DeleteAsync(se => se.SessionId == sessionId);
        });
    }

    public async Task<List<CalendarSessionIndicator>> GetAllSessionsForMonthAsync(int year, int month)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var allSessions = await db.Table<Session>().ToListAsync();
            var sessions = allSessions
                .Where(s => s.Date.Year == year && s.Date.Month == month)
                .ToList();

            if (sessions.Count == 0)
                return new List<CalendarSessionIndicator>();

            var programs = await db.Table<Program>().ToListAsync();
            var programDict = programs.ToDictionary(p => p.Id);

            var sessionExercises = await db.Table<SessionExercise>().ToListAsync();
            var sets = await db.Table<Set>().ToListAsync();

            return sessions.Select(s =>
            {
                var seIds = sessionExercises
                    .Where(se => se.SessionId == s.Id)
                    .Select(se => se.Id)
                    .ToList();

                Program? program = s.ProgramId.HasValue && programDict.TryGetValue(s.ProgramId.Value, out var p)
                    ? p : null;

                return new CalendarSessionIndicator
                {
                    SessionId = s.Id,
                    ProgramId = s.ProgramId,
                    ProgramName = program?.Name,
                    ProgramColor = program?.Color,
                    Date = s.Date,
                    ExerciseCount = seIds.Count,
                    SetCount = sets.Count(st => seIds.Contains(st.SessionExerciseId))
                };
            }).ToList();
        });
    }

    // ── Body Metric methods ──

    public async Task<int> SaveBodyMetricAsync(BodyMetric metric)
    {
        return await Task.Run(async () =>
        {
            await AppDatabase.Database.InsertAsync(metric);
            return metric.Id;
        });
    }

    public async Task<BodyMetric?> GetBodyMetricByIdAsync(int id)
    {
        return await Task.Run(async () =>
            await AppDatabase.Database.Table<BodyMetric>().FirstOrDefaultAsync(m => m.Id == id));
    }

    public async Task UpdateBodyMetricAsync(BodyMetric metric)
    {
        await Task.Run(async () =>
            await AppDatabase.Database.UpdateAsync(metric));
    }

    public async Task DeleteBodyMetricAsync(int id)
    {
        await Task.Run(async () =>
            await AppDatabase.Database.DeleteAsync(new BodyMetric { Id = id }));
    }

    // ── Recovery Log methods ──

    public async Task<int> SaveRecoveryLogAsync(RecoveryLog log)
    {
        return await Task.Run(async () =>
        {
            await AppDatabase.Database.InsertAsync(log);
            return log.Id;
        });
    }

    public async Task<RecoveryLog?> GetRecoveryLogByIdAsync(int id)
    {
        return await Task.Run(async () =>
            await AppDatabase.Database.Table<RecoveryLog>().FirstOrDefaultAsync(r => r.Id == id));
    }

    public async Task UpdateRecoveryLogAsync(RecoveryLog log)
    {
        await Task.Run(async () =>
            await AppDatabase.Database.UpdateAsync(log));
    }

    public async Task DeleteRecoveryLogAsync(int id)
    {
        await Task.Run(async () =>
            await AppDatabase.Database.DeleteAsync(new RecoveryLog { Id = id }));
    }

    // ── Calorie Log methods ──

    public async Task<int> SaveCalorieLogAsync(CalorieLog log)
    {
        return await Task.Run(async () =>
        {
            await AppDatabase.Database.InsertAsync(log);
            return log.Id;
        });
    }

    public async Task<CalorieLog?> GetCalorieLogByIdAsync(int id)
    {
        return await Task.Run(async () =>
            await AppDatabase.Database.Table<CalorieLog>().FirstOrDefaultAsync(c => c.Id == id));
    }

    public async Task UpdateCalorieLogAsync(CalorieLog log)
    {
        await Task.Run(async () =>
            await AppDatabase.Database.UpdateAsync(log));
    }

    public async Task DeleteCalorieLogAsync(int id)
    {
        await Task.Run(async () =>
            await AppDatabase.Database.DeleteAsync(new CalorieLog { Id = id }));
    }

    // ── All Logs ──

    public async Task<List<LogEntry>> GetAllLogsAsync()
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var entries = new List<LogEntry>();

            var bodyMetrics = await db.Table<BodyMetric>().ToListAsync();
            foreach (var m in bodyMetrics)
            {
                var parts = new List<string>();
                if (m.Bodyweight.HasValue) parts.Add($"{m.Bodyweight:0.#} kg");
                if (m.BodyFat.HasValue) parts.Add($"{m.BodyFat:0.#}% BF");
                entries.Add(new LogEntry
                {
                    Id = m.Id,
                    Date = m.Date,
                    LogType = "body_metric",
                    Summary = parts.Count > 0 ? string.Join(" \u00b7 ", parts) : "Body metric logged"
                });
            }

            var recoveryLogs = await db.Table<RecoveryLog>().ToListAsync();
            foreach (var r in recoveryLogs)
            {
                var parts = new List<string>();
                if (r.SleepHours.HasValue) parts.Add($"{r.SleepHours:0.#}h sleep");
                if (r.SorenessLevel.HasValue) parts.Add($"Soreness {r.SorenessLevel}/5");
                if (r.StressLevel.HasValue) parts.Add($"Stress {r.StressLevel}/5");
                entries.Add(new LogEntry
                {
                    Id = r.Id,
                    Date = r.Date,
                    LogType = "recovery",
                    Summary = parts.Count > 0 ? string.Join(" \u00b7 ", parts) : "Recovery logged"
                });
            }

            var calorieLogs = await db.Table<CalorieLog>().ToListAsync();
            foreach (var c in calorieLogs)
            {
                var parts = new List<string>();
                if (c.TotalCalories.HasValue) parts.Add($"{c.TotalCalories} cal");
                if (!string.IsNullOrEmpty(c.ActivityLevel)) parts.Add(c.ActivityLevel);
                entries.Add(new LogEntry
                {
                    Id = c.Id,
                    Date = c.Date,
                    LogType = "calorie",
                    Summary = parts.Count > 0 ? string.Join(" \u00b7 ", parts) : "Calories logged"
                });
            }

            return entries.OrderByDescending(e => e.Date).ToList();
        });
    }

    // ── Dashboard methods ──

    public async Task<List<DashboardSessionDisplay>> GetSessionsForDateAsync(DateTime date)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var allSessions = await db.Table<Session>().ToListAsync();
            var sessions = allSessions.Where(s => s.Date.Date == date.Date).ToList();

            if (sessions.Count == 0)
                return new List<DashboardSessionDisplay>();

            var programs = await db.Table<Program>().ToListAsync();
            var programDict = programs.ToDictionary(p => p.Id);
            var sessionExercises = await db.Table<SessionExercise>().ToListAsync();
            var sets = await db.Table<Set>().ToListAsync();

            return sessions.Select(s =>
            {
                var seIds = sessionExercises
                    .Where(se => se.SessionId == s.Id)
                    .Select(se => se.Id)
                    .ToList();

                Program? program = s.ProgramId.HasValue && programDict.TryGetValue(s.ProgramId.Value, out var p)
                    ? p : null;

                return new DashboardSessionDisplay
                {
                    SessionId = s.Id,
                    ProgramId = s.ProgramId,
                    ProgramName = program?.Name,
                    ProgramColor = program?.Color,
                    Date = s.Date,
                    ExerciseCount = seIds.Count,
                    SetCount = sets.Count(st => seIds.Contains(st.SessionExerciseId)),
                    IsCompleted = s.IsCompleted,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                };
            }).ToList();
        });
    }

    public async Task<List<DashboardSessionDisplay>> GetUpcomingSessionsAsync(int days = 7)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var tomorrow = DateTime.Today.AddDays(1);
            var endDate = DateTime.Today.AddDays(days);

            var allSessions = await db.Table<Session>().ToListAsync();
            var sessions = allSessions
                .Where(s => s.Date.Date >= tomorrow && s.Date.Date <= endDate)
                .OrderBy(s => s.Date)
                .ToList();

            if (sessions.Count == 0)
                return new List<DashboardSessionDisplay>();

            var programs = await db.Table<Program>().ToListAsync();
            var programDict = programs.ToDictionary(p => p.Id);
            var sessionExercises = await db.Table<SessionExercise>().ToListAsync();
            var sets = await db.Table<Set>().ToListAsync();

            return sessions.Select(s =>
            {
                var seIds = sessionExercises
                    .Where(se => se.SessionId == s.Id)
                    .Select(se => se.Id)
                    .ToList();

                Program? program = s.ProgramId.HasValue && programDict.TryGetValue(s.ProgramId.Value, out var p)
                    ? p : null;

                return new DashboardSessionDisplay
                {
                    SessionId = s.Id,
                    ProgramId = s.ProgramId,
                    ProgramName = program?.Name,
                    ProgramColor = program?.Color,
                    Date = s.Date,
                    ExerciseCount = seIds.Count,
                    SetCount = sets.Count(st => seIds.Contains(st.SessionExerciseId)),
                    IsCompleted = s.IsCompleted,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                };
            }).ToList();
        });
    }

    public async Task<(int WorkoutCount, int TotalSets, double TotalVolume)> GetWeeklyStatsAsync()
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var today = DateTime.Today;
            int diff = ((int)today.DayOfWeek - 1 + 7) % 7;
            var monday = today.AddDays(-diff);

            var allSessions = await db.Table<Session>().ToListAsync();
            var completedSessions = allSessions
                .Where(s => s.IsCompleted && s.Date.Date >= monday && s.Date.Date <= today)
                .ToList();

            var workoutCount = completedSessions.Count;

            var sessionIds = completedSessions.Select(s => s.Id).ToHashSet();
            var sessionExercises = await db.Table<SessionExercise>().ToListAsync();
            var relevantSeIds = sessionExercises
                .Where(se => sessionIds.Contains(se.SessionId))
                .Select(se => se.Id)
                .ToHashSet();

            var sets = await db.Table<Set>().ToListAsync();
            var completedSets = sets
                .Where(s => relevantSeIds.Contains(s.SessionExerciseId) && s.Completed)
                .ToList();

            var totalSets = completedSets.Count;
            var totalVolume = completedSets
                .Where(s => s.Reps.HasValue && s.Weight.HasValue)
                .Sum(s => s.Reps!.Value * s.Weight!.Value);

            return (workoutCount, totalSets, totalVolume);
        });
    }

    public async Task<List<ActiveExerciseDisplay>> GetActiveWorkoutDataAsync(int sessionId)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var sessionExercises = await db.Table<SessionExercise>()
                .Where(se => se.SessionId == sessionId)
                .OrderBy(se => se.Order)
                .ToListAsync();

            var exercises = await db.Table<Exercise>().ToListAsync();
            var exerciseMuscles = await db.Table<ExerciseMuscle>().ToListAsync();
            var muscles = await db.Table<Muscle>().ToListAsync();
            var sets = await db.Table<Set>().ToListAsync();

            var muscleDict = muscles.ToDictionary(m => m.Id, m => m.Name);
            var exerciseDict = exercises.ToDictionary(e => e.Id);

            return sessionExercises.Select(se =>
            {
                exerciseDict.TryGetValue(se.ExerciseId, out var ex);
                var primaryMuscle = ex != null
                    ? exerciseMuscles
                        .Where(em => em.ExerciseId == ex.Id && em.Type == "primary")
                        .Select(em => muscleDict.GetValueOrDefault(em.MuscleId))
                        .FirstOrDefault()
                    : null;

                var display = new ActiveExerciseDisplay
                {
                    SessionExerciseId = se.Id,
                    ExerciseId = se.ExerciseId,
                    ExerciseName = ex?.Name ?? "Unknown",
                    ExerciseType = ex?.ExerciseType,
                    PrimaryMuscle = primaryMuscle,
                    IsTimeBased = ex?.IsTimeBased ?? false,
                    Order = se.Order,
                    Notes = se.Notes,
                    RestSeconds = se.RestSeconds ?? 120
                };

                var exerciseSets = sets
                    .Where(s => s.SessionExerciseId == se.Id)
                    .OrderBy(s => s.SetNumber)
                    .Select(s => new ActiveSetDisplay
                    {
                        SetId = s.Id,
                        SetNumber = s.SetNumber,
                        IsWarmup = s.IsWarmup,
                        PlannedRepMin = s.RepMin,
                        PlannedRepMax = s.RepMax,
                        PlannedDurationMin = s.DurationMin,
                        PlannedDurationMax = s.DurationMax,
                        RepsText = s.Reps?.ToString() ?? "",
                        WeightText = s.Weight?.ToString() ?? "",
                        DurationText = s.DurationSeconds?.ToString() ?? "",
                        RpeText = s.Rpe?.ToString() ?? ""
                    });

                foreach (var set in exerciseSets)
                    display.Sets.Add(set);

                return display;
            }).ToList();
        });
    }

    public async Task SaveActiveWorkoutSetsAsync(List<ActiveExerciseDisplay> exercises)
    {
        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            foreach (var ex in exercises)
            {
                foreach (var set in ex.Sets)
                {
                    int.TryParse(set.RepsText, out var reps);
                    double.TryParse(set.WeightText, out var weight);
                    int.TryParse(set.DurationText, out var duration);
                    double.TryParse(set.RpeText, out var rpe);

                    var dbSet = await db.Table<Set>().FirstOrDefaultAsync(s => s.Id == set.SetId);
                    if (dbSet != null)
                    {
                        dbSet.Reps = reps > 0 ? reps : null;
                        dbSet.Weight = weight > 0 ? weight : null;
                        dbSet.DurationSeconds = duration > 0 ? duration : null;
                        dbSet.Rpe = rpe > 0 ? rpe : null;
                        dbSet.Completed = reps > 0 || duration > 0;
                        await db.UpdateAsync(dbSet);
                    }
                }
            }
        });
    }

    public async Task OffsetSessionDatesAsync(int programId, TimeSpan offset)
    {
        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var sessions = await db.Table<Session>()
                .Where(s => s.ProgramId == programId)
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.Date += offset;
                await db.UpdateAsync(session);
            }
        });
    }

    // ── Duplicate Program ──

    public async Task<int> DuplicateProgramAsync(int sourceProgramId, string newName, DateTime newStartDate)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;

            Program? source = await db.Table<Program>().FirstOrDefaultAsync(p => p.Id == sourceProgramId);
            if (source == null) return -1;

            var dateOffset = newStartDate - source.StartDate;

            Program newProgram = new Program
            {
                Name = newName,
                Goal = source.Goal,
                StartDate = newStartDate,
                EndDate = source.EndDate.HasValue ? source.EndDate.Value + dateOffset : null,
                Notes = source.Notes,
                Color = source.Color
            };
            await db.InsertAsync(newProgram);

            var sessions = await db.Table<Session>()
                .Where(s => s.ProgramId == sourceProgramId)
                .ToListAsync();

            foreach (var session in sessions)
            {
                var newSession = new Session
                {
                    ProgramId = newProgram.Id,
                    Date = session.Date + dateOffset,
                    Notes = session.Notes,
                    Week = session.Week,
                    Day = session.Day,
                    IsCompleted = false
                };
                await db.InsertAsync(newSession);

                var sessionExercises = await db.Table<SessionExercise>()
                    .Where(se => se.SessionId == session.Id)
                    .OrderBy(se => se.Order)
                    .ToListAsync();

                foreach (var se in sessionExercises)
                {
                    var newSe = new SessionExercise
                    {
                        SessionId = newSession.Id,
                        ExerciseId = se.ExerciseId,
                        Order = se.Order,
                        Notes = se.Notes,
                        RestSeconds = se.RestSeconds
                    };
                    await db.InsertAsync(newSe);

                    var sets = await db.Table<Set>()
                        .Where(s => s.SessionExerciseId == se.Id)
                        .OrderBy(s => s.SetNumber)
                        .ToListAsync();

                    foreach (var set in sets)
                    {
                        var newSet = new Set
                        {
                            SessionExerciseId = newSe.Id,
                            SetNumber = set.SetNumber,
                            IsWarmup = set.IsWarmup,
                            RepMin = set.RepMin,
                            RepMax = set.RepMax,
                            DurationMin = set.DurationMin,
                            DurationMax = set.DurationMax,
                            Completed = false
                        };
                        await db.InsertAsync(newSet);
                    }
                }
            }

            return newProgram.Id;
        });
    }

    // ── Reset Program Completion ──

    public async Task ResetProgramCompletionAsync(int programId)
    {
        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var sessions = await db.Table<Session>()
                .Where(s => s.ProgramId == programId)
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.IsCompleted = false;
                session.StartTime = null;
                session.EndTime = null;
                session.EnergyLevel = null;
                await db.UpdateAsync(session);

                var sessionExercises = await db.Table<SessionExercise>()
                    .Where(se => se.SessionId == session.Id)
                    .ToListAsync();

                foreach (var se in sessionExercises)
                {
                    var sets = await db.Table<Set>()
                        .Where(s => s.SessionExerciseId == se.Id)
                        .ToListAsync();

                    foreach (var set in sets)
                    {
                        set.Completed = false;
                        set.Reps = null;
                        set.Weight = null;
                        set.DurationSeconds = null;
                        set.Rpe = null;
                        await db.UpdateAsync(set);
                    }
                }
            }
        });
    }

    // ── Analytics methods ──

    public async Task<AnalyticsSummary> GetAnalyticsSummaryAsync(DateTime from, DateTime to)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var allSessions = await db.Table<Session>().ToListAsync();
            var completedSessions = allSessions
                .Where(s => s.IsCompleted && s.Date.Date >= from.Date && s.Date.Date <= to.Date)
                .ToList();

            var sessionIds = completedSessions.Select(s => s.Id).ToHashSet();
            var sessionExercises = await db.Table<SessionExercise>().ToListAsync();
            var relevantSe = sessionExercises.Where(se => sessionIds.Contains(se.SessionId)).ToList();
            var relevantSeIds = relevantSe.Select(se => se.Id).ToHashSet();

            var sets = await db.Table<Set>().ToListAsync();
            var completedSets = sets
                .Where(s => relevantSeIds.Contains(s.SessionExerciseId) && s.Completed)
                .ToList();

            var totalVolume = completedSets
                .Where(s => s.Reps.HasValue && s.Weight.HasValue)
                .Sum(s => s.Reps!.Value * s.Weight!.Value);

            var durationsMinutes = completedSessions
                .Where(s => s.StartTime.HasValue && s.EndTime.HasValue)
                .Select(s => (s.EndTime!.Value - s.StartTime!.Value).TotalMinutes)
                .ToList();

            var energyLevels = completedSessions
                .Where(s => s.EnergyLevel.HasValue)
                .Select(s => (double)s.EnergyLevel!.Value)
                .ToList();

            return new AnalyticsSummary
            {
                TotalSessions = completedSessions.Count,
                TotalExercises = relevantSe.Select(se => se.ExerciseId).Distinct().Count(),
                TotalSetsCompleted = completedSets.Count,
                TotalVolume = totalVolume,
                AvgSessionDurationMinutes = durationsMinutes.Count > 0 ? durationsMinutes.Average() : 0,
                AvgEnergyLevel = energyLevels.Count > 0 ? energyLevels.Average() : 0
            };
        });
    }

    public async Task<List<MuscleVolumeData>> GetMuscleVolumeAsync(DateTime from, DateTime to)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var allSessions = await db.Table<Session>().ToListAsync();
            var completedSessionIds = allSessions
                .Where(s => s.IsCompleted && s.Date.Date >= from.Date && s.Date.Date <= to.Date)
                .Select(s => s.Id)
                .ToHashSet();

            var sessionExercises = await db.Table<SessionExercise>().ToListAsync();
            var relevantSe = sessionExercises.Where(se => completedSessionIds.Contains(se.SessionId)).ToList();
            var relevantSeIds = relevantSe.Select(se => se.Id).ToHashSet();

            var sets = await db.Table<Set>().ToListAsync();
            var completedSets = sets
                .Where(s => relevantSeIds.Contains(s.SessionExerciseId) && s.Completed && !s.IsWarmup)
                .ToList();

            // Build a map: sessionExerciseId -> exerciseId
            var seToExercise = relevantSe.ToDictionary(se => se.Id, se => se.ExerciseId);

            var exerciseMuscles = await db.Table<ExerciseMuscle>().ToListAsync();
            var muscles = await db.Table<Muscle>().ToListAsync();
            var muscleDict = muscles.ToDictionary(m => m.Id, m => m.Name);

            var muscleVolume = new Dictionary<string, (double Sets, double Volume)>();

            foreach (var set in completedSets)
            {
                if (!seToExercise.TryGetValue(set.SessionExerciseId, out var exerciseId))
                    continue;

                var eMuscles = exerciseMuscles.Where(em => em.ExerciseId == exerciseId).ToList();
                var volume = (set.Reps ?? 0) * (set.Weight ?? 0);

                foreach (var em in eMuscles)
                {
                    var name = muscleDict.GetValueOrDefault(em.MuscleId, "Unknown");
                    double weight = em.Type == "primary" ? 1.0 : 0.5;

                    if (!muscleVolume.ContainsKey(name))
                        muscleVolume[name] = (0, 0);

                    var current = muscleVolume[name];
                    muscleVolume[name] = (current.Sets + weight, current.Volume + volume * weight);
                }
            }

            return muscleVolume
                .Select(kv => new MuscleVolumeData
                {
                    MuscleName = kv.Key,
                    TotalSets = kv.Value.Sets,
                    TotalVolume = kv.Value.Volume
                })
                .OrderByDescending(m => m.TotalSets)
                .ToList();
        });
    }

    public async Task<List<ExerciseProgressionPoint>> GetExerciseProgressionAsync(int exerciseId, DateTime from, DateTime to)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var allSessions = await db.Table<Session>().ToListAsync();
            var completedSessions = allSessions
                .Where(s => s.IsCompleted && s.Date.Date >= from.Date && s.Date.Date <= to.Date)
                .ToDictionary(s => s.Id);

            var sessionExercises = await db.Table<SessionExercise>().ToListAsync();
            var relevantSe = sessionExercises
                .Where(se => se.ExerciseId == exerciseId && completedSessions.ContainsKey(se.SessionId))
                .ToList();

            var relevantSeIds = relevantSe.Select(se => se.Id).ToHashSet();
            var sets = await db.Table<Set>().ToListAsync();
            var relevantSets = sets
                .Where(s => relevantSeIds.Contains(s.SessionExerciseId) && s.Completed && !s.IsWarmup)
                .ToList();

            // Group by session date
            var seToSession = relevantSe.ToDictionary(se => se.Id, se => se.SessionId);

            // Get program goals for context
            var programs = await db.Table<Program>().ToListAsync();
            var programDict = programs.ToDictionary(p => p.Id);

            var grouped = relevantSets
                .GroupBy(s =>
                {
                    var sessionId = seToSession[s.SessionExerciseId];
                    return completedSessions[sessionId].Date.Date;
                })
                .OrderBy(g => g.Key);

            return grouped.Select(g =>
            {
                var sessionSets = g.ToList();
                var bestWeight = sessionSets.Where(s => s.Weight.HasValue).Max(s => s.Weight) ?? 0;
                var bestSet = sessionSets
                    .Where(s => s.Weight.HasValue && s.Reps.HasValue && s.Weight.Value > 0 && s.Reps.Value <= 10)
                    .OrderByDescending(s => s.Weight!.Value * (1 + s.Reps!.Value / 30.0))
                    .FirstOrDefault();
                var e1rm = bestSet != null
                    ? bestSet.Weight!.Value * (1 + bestSet.Reps!.Value / 30.0)
                    : 0;

                var totalVolume = sessionSets
                    .Where(s => s.Reps.HasValue && s.Weight.HasValue)
                    .Sum(s => s.Reps!.Value * s.Weight!.Value);

                // Find program goal
                var sessionId = seToSession[sessionSets.First().SessionExerciseId];
                var session = completedSessions[sessionId];
                string? goal = null;
                if (session.ProgramId.HasValue && programDict.TryGetValue(session.ProgramId.Value, out var prog))
                    goal = prog.Goal;

                return new ExerciseProgressionPoint
                {
                    Date = g.Key,
                    BestWeight = bestWeight,
                    EstimatedOneRepMax = Math.Round(e1rm, 1),
                    TotalVolume = totalVolume,
                    TotalSets = sessionSets.Count,
                    TotalReps = sessionSets.Where(s => s.Reps.HasValue).Sum(s => s.Reps!.Value),
                    ProgramGoal = goal
                };
            }).ToList();
        });
    }

    public async Task<List<SessionAdherenceData>> GetSessionAdherenceAsync(DateTime from, DateTime to)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var allSessions = await db.Table<Session>().ToListAsync();
            var sessions = allSessions
                .Where(s => s.Date.Date >= from.Date && s.Date.Date <= to.Date)
                .OrderBy(s => s.Date)
                .ToList();

            var sessionExercises = await db.Table<SessionExercise>().ToListAsync();
            var sets = await db.Table<Set>().ToListAsync();

            return sessions.Select(s =>
            {
                var seIds = sessionExercises
                    .Where(se => se.SessionId == s.Id)
                    .Select(se => se.Id)
                    .ToHashSet();

                var sessionSets = sets
                    .Where(st => seIds.Contains(st.SessionExerciseId) && !st.IsWarmup)
                    .ToList();

                return new SessionAdherenceData
                {
                    Date = s.Date,
                    PlannedSets = sessionSets.Count,
                    CompletedSets = sessionSets.Count(st => st.Completed),
                    SessionCompleted = s.IsCompleted
                };
            }).ToList();
        });
    }

    public async Task<List<BodyMetricPoint>> GetBodyMetricTrendAsync(DateTime from, DateTime to)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var allMetrics = await db.Table<BodyMetric>().ToListAsync();
            return allMetrics
                .Where(m => m.Date.Date >= from.Date && m.Date.Date <= to.Date)
                .OrderBy(m => m.Date)
                .Select(m => new BodyMetricPoint
                {
                    Date = m.Date,
                    Bodyweight = m.Bodyweight,
                    BodyFat = m.BodyFat
                })
                .ToList();
        });
    }

    public async Task<List<ExercisePickerItem>> GetExercisesWithCompletedSetsAsync()
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var sets = await db.Table<Set>().ToListAsync();
            var completedSeIds = sets
                .Where(s => s.Completed)
                .Select(s => s.SessionExerciseId)
                .Distinct()
                .ToHashSet();

            var sessionExercises = await db.Table<SessionExercise>().ToListAsync();
            var exerciseIds = sessionExercises
                .Where(se => completedSeIds.Contains(se.Id))
                .Select(se => se.ExerciseId)
                .Distinct()
                .ToHashSet();

            var exercises = await db.Table<Exercise>().ToListAsync();
            return exercises
                .Where(e => exerciseIds.Contains(e.Id))
                .OrderBy(e => e.Name)
                .Select(e => new ExercisePickerItem { Id = e.Id, Name = e.Name })
                .ToList();
        });
    }

    public async Task DeleteExerciseFromSessionsAsync(int exerciseId)
    {
        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var rows = await db.Table<SessionExercise>()
                .Where(se => se.ExerciseId == exerciseId).ToListAsync();

            var affectedSessionIds = rows.Select(r => r.SessionId).Distinct().ToList();

            // Delete sets for affected session exercises, then delete the session exercises
            foreach (var row in rows)
            {
                await db.Table<Set>().DeleteAsync(s => s.SessionExerciseId == row.Id);
                await db.DeleteAsync(row);
            }

            // Re-order remaining exercises in affected sessions
            foreach (var sessionId in affectedSessionIds)
            {
                var remaining = await db.Table<SessionExercise>()
                    .Where(se => se.SessionId == sessionId)
                    .OrderBy(se => se.Order)
                    .ToListAsync();

                for (int i = 0; i < remaining.Count; i++)
                {
                    if (remaining[i].Order != i + 1)
                    {
                        remaining[i].Order = i + 1;
                        await db.UpdateAsync(remaining[i]);
                    }
                }
            }
        });
    }
}
