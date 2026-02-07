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
}
