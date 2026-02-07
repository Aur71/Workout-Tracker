using Workout_Tracker.Model;

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
}
