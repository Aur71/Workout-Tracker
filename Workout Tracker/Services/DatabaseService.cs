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
        await DeleteExerciseFromWorkoutsAsync(id);
        await DeleteExerciseFromSessionsAsync(id);

        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            await db.Table<ExerciseMuscle>().DeleteAsync(em => em.ExerciseId == id);
            await db.DeleteAsync(new Exercise { Id = id });
        });
    }

    // ── Workout methods ──

    public async Task<List<WorkoutDisplay>> GetAllWorkoutsAsync()
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var workouts = await db.Table<Workout>().ToListAsync();
            var workoutExercises = await db.Table<WorkoutExercise>().ToListAsync();
            var exercises = await db.Table<Exercise>().ToListAsync();
            var exerciseMuscles = await db.Table<ExerciseMuscle>().ToListAsync();
            var muscles = await db.Table<Muscle>().ToListAsync();

            var muscleDict = muscles.ToDictionary(m => m.Id, m => m.Name);
            var exerciseDict = exercises.ToDictionary(e => e.Id);

            return workouts.Select(w =>
            {
                var wExercises = workoutExercises
                    .Where(we => we.WorkoutId == w.Id)
                    .OrderBy(we => we.Order)
                    .Select(we =>
                    {
                        exerciseDict.TryGetValue(we.ExerciseId, out var ex);
                        var primaryMuscle = ex != null
                            ? exerciseMuscles
                                .Where(em => em.ExerciseId == ex.Id && em.Type == "primary")
                                .Select(em => muscleDict.GetValueOrDefault(em.MuscleId))
                                .FirstOrDefault()
                            : null;

                        return new WorkoutExerciseDisplay
                        {
                            ExerciseId = we.ExerciseId,
                            ExerciseName = ex?.Name ?? "Unknown",
                            Order = we.Order,
                            ExerciseType = ex?.ExerciseType,
                            PrimaryMuscle = primaryMuscle
                        };
                    })
                    .ToList();

                return new WorkoutDisplay
                {
                    Id = w.Id,
                    Name = w.Name,
                    Goal = w.Goal,
                    EstimatedDuration = w.EstimatedDuration,
                    Notes = w.Notes,
                    Exercises = wExercises
                };
            }).ToList();
        });
    }

    public async Task<WorkoutDisplay?> GetWorkoutByIdAsync(int id)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var workout = await db.Table<Workout>().FirstOrDefaultAsync(w => w.Id == id);
            if (workout == null) return null;

            var workoutExercises = await db.Table<WorkoutExercise>()
                .Where(we => we.WorkoutId == id).ToListAsync();
            var exercises = await db.Table<Exercise>().ToListAsync();
            var exerciseMuscles = await db.Table<ExerciseMuscle>().ToListAsync();
            var muscles = await db.Table<Muscle>().ToListAsync();

            var muscleDict = muscles.ToDictionary(m => m.Id, m => m.Name);
            var exerciseDict = exercises.ToDictionary(e => e.Id);

            var wExercises = workoutExercises
                .OrderBy(we => we.Order)
                .Select(we =>
                {
                    exerciseDict.TryGetValue(we.ExerciseId, out var ex);
                    var primaryMuscle = ex != null
                        ? exerciseMuscles
                            .Where(em => em.ExerciseId == ex.Id && em.Type == "primary")
                            .Select(em => muscleDict.GetValueOrDefault(em.MuscleId))
                            .FirstOrDefault()
                        : null;

                    return new WorkoutExerciseDisplay
                    {
                        ExerciseId = we.ExerciseId,
                        ExerciseName = ex?.Name ?? "Unknown",
                        Order = we.Order,
                        ExerciseType = ex?.ExerciseType,
                        PrimaryMuscle = primaryMuscle
                    };
                })
                .ToList();

            return new WorkoutDisplay
            {
                Id = workout.Id,
                Name = workout.Name,
                Goal = workout.Goal,
                EstimatedDuration = workout.EstimatedDuration,
                Notes = workout.Notes,
                Exercises = wExercises
            };
        });
    }

    public async Task<int> SaveWorkoutAsync(Workout workout)
    {
        return await Task.Run(async () =>
        {
            await AppDatabase.Database.InsertAsync(workout);
            return workout.Id;
        });
    }

    public async Task UpdateWorkoutAsync(Workout workout)
    {
        await Task.Run(async () =>
            await AppDatabase.Database.UpdateAsync(workout));
    }

    public async Task SaveWorkoutExercisesAsync(int workoutId, List<ExerciseSelection> exercises)
    {
        var rows = exercises
            .Where(e => e.IsSelected)
            .Select((e, i) => new WorkoutExercise
            {
                WorkoutId = workoutId,
                ExerciseId = e.Exercise.Id,
                Order = i + 1
            }).ToList();

        await Task.Run(async () =>
            await AppDatabase.Database.InsertAllAsync(rows));
    }

    public async Task DeleteWorkoutExercisesAsync(int workoutId)
    {
        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var rows = await db.Table<WorkoutExercise>()
                .Where(we => we.WorkoutId == workoutId).ToListAsync();
            foreach (var row in rows)
                await db.DeleteAsync(row);
        });
    }

    public async Task DeleteWorkoutAsync(int id)
    {
        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            await db.Table<WorkoutExercise>().DeleteAsync(we => we.WorkoutId == id);
            await db.DeleteAsync(new Workout { Id = id });
        });
    }

    public async Task<int> GetWorkoutCountForExerciseAsync(int exerciseId)
    {
        return await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            return await db.Table<WorkoutExercise>()
                .Where(we => we.ExerciseId == exerciseId)
                .CountAsync();
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
            await AppDatabase.Database.DeleteAsync(new Program { Id = id }));
    }

    public async Task DeleteExerciseFromWorkoutsAsync(int exerciseId)
    {
        await Task.Run(async () =>
        {
            var db = AppDatabase.Database;
            var rows = await db.Table<WorkoutExercise>()
                .Where(we => we.ExerciseId == exerciseId).ToListAsync();

            var affectedWorkoutIds = rows.Select(r => r.WorkoutId).Distinct().ToList();

            foreach (var row in rows)
                await db.DeleteAsync(row);

            // Re-order remaining exercises in affected workouts
            foreach (var workoutId in affectedWorkoutIds)
            {
                var remaining = await db.Table<WorkoutExercise>()
                    .Where(we => we.WorkoutId == workoutId)
                    .OrderBy(we => we.Order)
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
                    SetCount = sets.Count(st => seIds.Contains(st.SessionExerciseId))
                };
            }).ToList();
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
                    Notes = se.Notes
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
                var sessionExercise = new SessionExercise
                {
                    SessionId = sessionId,
                    ExerciseId = ex.ExerciseId,
                    Order = ex.Order,
                    Notes = ex.Notes
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
