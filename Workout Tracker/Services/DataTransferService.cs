using System.Text.Json;
using Workout_Tracker.Model;
using Program = Workout_Tracker.Model.Program;

namespace Workout_Tracker.Services;

public class DataTransferService
{
    private const int CurrentVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public async Task<string> ExportAsync()
    {
        var db = AppDatabase.Database;

        var exportData = new ExportData
        {
            Version = CurrentVersion,
            ExportDate = DateTime.UtcNow,
            Data = new ExportPayload
            {
                Tags = await db.Table<Tag>().ToListAsync(),
                Muscles = await db.Table<Muscle>().ToListAsync(),
                Exercises = await db.Table<Exercise>().ToListAsync(),
                ExerciseMuscles = await db.Table<ExerciseMuscle>().ToListAsync(),
                Programs = await db.Table<Program>().ToListAsync(),
                Sessions = await db.Table<Session>().ToListAsync(),
                SessionExercises = await db.Table<SessionExercise>().ToListAsync(),
                Sets = await db.Table<Set>().ToListAsync(),
                BodyMetrics = await db.Table<BodyMetric>().ToListAsync(),
                RecoveryLogs = await db.Table<RecoveryLog>().ToListAsync(),
                CalorieLogs = await db.Table<CalorieLog>().ToListAsync()
            }
        };

        var json = JsonSerializer.Serialize(exportData, JsonOptions);
        var fileName = $"workout_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllTextAsync(filePath, json);

        return filePath;
    }

    public (bool IsValid, string? ErrorMessage) ValidateImport(ExportData? data)
    {
        if (data == null)
            return (false, "This file is not a valid backup file.");

        if (data.Type is not null and not "full_backup")
            return (false, "This file is a program export, not a full backup. To import a program, use the Import option on the Programs page.");

        if (data.Version <= 0 || data.Version > CurrentVersion)
            return (false, "This backup was created by a newer version of the app. Please update the app first.");

        if (data.Data == null)
            return (false, "This backup file is empty or corrupted.");

        return (true, null);
    }

    // ── Program Export/Import ──

    public async Task<string> ExportProgramAsync(int programId)
    {
        var db = AppDatabase.Database;

        var program = await db.Table<Program>().FirstOrDefaultAsync(p => p.Id == programId);
        if (program == null)
            throw new InvalidOperationException("Program not found.");

        var sessions = await db.Table<Session>()
            .Where(s => s.ProgramId == programId)
            .ToListAsync();

        var sessionIds = sessions.Select(s => s.Id).ToHashSet();

        var allSessionExercises = await db.Table<SessionExercise>().ToListAsync();
        var sessionExercises = allSessionExercises
            .Where(se => sessionIds.Contains(se.SessionId))
            .ToList();

        var seIds = sessionExercises.Select(se => se.Id).ToHashSet();

        var allSets = await db.Table<Set>().ToListAsync();
        var sets = allSets
            .Where(s => seIds.Contains(s.SessionExerciseId))
            .ToList();

        // Collect referenced exercises
        var exerciseIds = sessionExercises.Select(se => se.ExerciseId).Distinct().ToHashSet();
        var allExercises = await db.Table<Exercise>().ToListAsync();
        var exercises = allExercises.Where(e => exerciseIds.Contains(e.Id)).ToList();

        // Collect referenced muscles via ExerciseMuscle
        var allExerciseMuscles = await db.Table<ExerciseMuscle>().ToListAsync();
        var exerciseMuscles = allExerciseMuscles
            .Where(em => exerciseIds.Contains(em.ExerciseId))
            .ToList();

        var muscleIds = exerciseMuscles.Select(em => em.MuscleId).Distinct().ToHashSet();
        var allMuscles = await db.Table<Muscle>().ToListAsync();
        var muscles = allMuscles.Where(m => muscleIds.Contains(m.Id)).ToList();

        // Strip completion data from sessions
        foreach (var s in sessions)
        {
            s.IsCompleted = false;
            s.StartTime = null;
            s.EndTime = null;
            s.EnergyLevel = null;
        }

        // Strip performed data from sets, keep only template
        foreach (var set in sets)
        {
            set.Reps = null;
            set.Weight = null;
            set.DurationSeconds = null;
            set.RestSeconds = null;
            set.Rpe = null;
            set.Completed = false;
        }

        var exportData = new ProgramExportData
        {
            Version = CurrentVersion,
            ExportDate = DateTime.UtcNow,
            Data = new ProgramExportPayload
            {
                Program = program,
                Sessions = sessions,
                SessionExercises = sessionExercises,
                Sets = sets,
                Exercises = exercises,
                Muscles = muscles,
                ExerciseMuscles = exerciseMuscles
            }
        };

        var json = JsonSerializer.Serialize(exportData, JsonOptions);
        var safeName = program.Name.Replace(" ", "_").Replace("/", "-");
        var fileName = $"program_{safeName}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllTextAsync(filePath, json);

        return filePath;
    }

    public async Task<int> ImportProgramAsync(Stream stream)
    {
        ProgramExportData? data;
        try
        {
            data = await JsonSerializer.DeserializeAsync<ProgramExportData>(stream, JsonOptions);
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("This file is not a valid program file.");
        }

        if (data == null || data.Data == null)
            throw new InvalidOperationException("This file is not a valid program file.");

        if (data.Type is not null and not "program")
            throw new InvalidOperationException("This file is a full backup, not a program export. To restore a full backup, use the Import option in Settings.");

        if (data.Version <= 0 || data.Version > CurrentVersion)
            throw new InvalidOperationException("This program was exported by a newer version of the app. Please update the app first.");

        if (string.IsNullOrWhiteSpace(data.Data.Program?.Name))
            throw new InvalidOperationException("This file is not a valid program file.");

        var payload = data.Data;
        payload.Sessions ??= [];
        payload.SessionExercises ??= [];
        payload.Sets ??= [];
        payload.Exercises ??= [];
        payload.Muscles ??= [];
        payload.ExerciseMuscles ??= [];

        var db = AppDatabase.Database;

        // 1. Match muscles by name (seeded data, should already exist)
        var existingMuscles = await db.Table<Muscle>().ToListAsync();
        var muscleNameToId = existingMuscles.ToDictionary(m => m.Name, m => m.Id);
        var oldMuscleIdMap = new Dictionary<int, int>(); // old -> new

        foreach (var muscle in payload.Muscles)
        {
            if (muscleNameToId.TryGetValue(muscle.Name, out var existingId))
            {
                oldMuscleIdMap[muscle.Id] = existingId;
            }
            else
            {
                var oldId = muscle.Id;
                muscle.Id = 0;
                await db.InsertAsync(muscle);
                oldMuscleIdMap[oldId] = muscle.Id;
            }
        }

        // 2. Match exercises by name - reuse existing, create new ones
        var existingExercises = await db.Table<Exercise>().ToListAsync();
        var exerciseNameToId = existingExercises.ToDictionary(e => e.Name, e => e.Id);
        var oldExerciseIdMap = new Dictionary<int, int>(); // old -> new
        var newlyCreatedExerciseIds = new HashSet<int>(); // track which exercises we created

        foreach (var exercise in payload.Exercises)
        {
            if (exerciseNameToId.TryGetValue(exercise.Name, out var existingId))
            {
                oldExerciseIdMap[exercise.Id] = existingId;
            }
            else
            {
                var oldId = exercise.Id;
                exercise.Id = 0;
                await db.InsertAsync(exercise);
                oldExerciseIdMap[oldId] = exercise.Id;
                newlyCreatedExerciseIds.Add(exercise.Id);
            }
        }

        // 3. Insert ExerciseMuscles only for newly created exercises
        foreach (var em in payload.ExerciseMuscles)
        {
            if (!oldExerciseIdMap.TryGetValue(em.ExerciseId, out var newExerciseId))
                continue;
            if (!newlyCreatedExerciseIds.Contains(newExerciseId))
                continue;
            if (!oldMuscleIdMap.TryGetValue(em.MuscleId, out var newMuscleId))
                continue;

            em.Id = 0;
            em.ExerciseId = newExerciseId;
            em.MuscleId = newMuscleId;
            await db.InsertAsync(em);
        }

        // 4. Insert Program with date offset to today
        var originalStartDate = payload.Program.StartDate;
        var dateOffset = DateTime.Today - originalStartDate.Date;

        var newProgram = new Program
        {
            Name = payload.Program.Name,
            Goal = payload.Program.Goal,
            StartDate = DateTime.Today,
            EndDate = payload.Program.EndDate.HasValue
                ? payload.Program.EndDate.Value + dateOffset
                : null,
            Notes = payload.Program.Notes,
            Color = payload.Program.Color
        };
        await db.InsertAsync(newProgram);

        // 5. Insert Sessions with remapped ProgramId and offset dates
        var oldSessionIdMap = new Dictionary<int, int>();
        foreach (var session in payload.Sessions)
        {
            var oldId = session.Id;
            session.Id = 0;
            session.ProgramId = newProgram.Id;
            session.Date = session.Date + dateOffset;
            session.IsCompleted = false;
            session.StartTime = null;
            session.EndTime = null;
            session.EnergyLevel = null;
            await db.InsertAsync(session);
            oldSessionIdMap[oldId] = session.Id;
        }

        // 6. Insert SessionExercises with remapped IDs
        var oldSeIdMap = new Dictionary<int, int>();
        foreach (var se in payload.SessionExercises)
        {
            if (!oldSessionIdMap.TryGetValue(se.SessionId, out var newSessionId))
                continue;
            if (!oldExerciseIdMap.TryGetValue(se.ExerciseId, out var newExerciseId))
                continue;

            var oldId = se.Id;
            se.Id = 0;
            se.SessionId = newSessionId;
            se.ExerciseId = newExerciseId;
            await db.InsertAsync(se);
            oldSeIdMap[oldId] = se.Id;
        }

        // 7. Insert Sets with remapped IDs (template data only)
        foreach (var set in payload.Sets)
        {
            if (!oldSeIdMap.TryGetValue(set.SessionExerciseId, out var newSeId))
                continue;

            set.Id = 0;
            set.SessionExerciseId = newSeId;
            set.Reps = null;
            set.Weight = null;
            set.DurationSeconds = null;
            set.Rpe = null;
            set.Completed = false;
            await db.InsertAsync(set);
        }

        return newProgram.Id;
    }

    public async Task ImportAsync(Stream stream)
    {
        ExportData? data;
        try
        {
            data = await JsonSerializer.DeserializeAsync<ExportData>(stream, JsonOptions);
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("This file is not a valid backup file.");
        }

        var (isValid, errorMessage) = ValidateImport(data);
        if (!isValid)
            throw new InvalidOperationException(errorMessage);

        // Default null collections to empty
        var payload = data!.Data!;
        payload.Tags ??= [];
        payload.Muscles ??= [];
        payload.Exercises ??= [];
        payload.ExerciseMuscles ??= [];
        payload.Programs ??= [];
        payload.Sessions ??= [];
        payload.SessionExercises ??= [];
        payload.Sets ??= [];
        payload.BodyMetrics ??= [];
        payload.RecoveryLogs ??= [];
        payload.CalorieLogs ??= [];

        var db = AppDatabase.Database;

        await db.RunInTransactionAsync(conn =>
        {
            // Delete all data (child tables first)
            conn.DeleteAll<Set>();
            conn.DeleteAll<SessionExercise>();
            conn.DeleteAll<Session>();
            conn.DeleteAll<ExerciseMuscle>();
            conn.DeleteAll<Exercise>();
            conn.DeleteAll<Muscle>();
            conn.DeleteAll<Tag>();
            conn.DeleteAll<Program>();
            conn.DeleteAll<BodyMetric>();
            conn.DeleteAll<RecoveryLog>();
            conn.DeleteAll<CalorieLog>();

            // Insert all data (parent tables first, preserving original IDs).
            // "OR REPLACE" forces sqlite-net to include the PK column in the
            // INSERT so the exported IDs are kept and foreign-key references
            // (e.g. Session.ProgramId) remain valid.
            conn.InsertAll(payload.Tags, "OR REPLACE", false);
            conn.InsertAll(payload.Muscles, "OR REPLACE", false);
            conn.InsertAll(payload.Exercises, "OR REPLACE", false);
            conn.InsertAll(payload.ExerciseMuscles, "OR REPLACE", false);
            conn.InsertAll(payload.Programs, "OR REPLACE", false);
            conn.InsertAll(payload.Sessions, "OR REPLACE", false);
            conn.InsertAll(payload.SessionExercises, "OR REPLACE", false);
            conn.InsertAll(payload.Sets, "OR REPLACE", false);
            conn.InsertAll(payload.BodyMetrics, "OR REPLACE", false);
            conn.InsertAll(payload.RecoveryLogs, "OR REPLACE", false);
            conn.InsertAll(payload.CalorieLogs, "OR REPLACE", false);
        });
    }
}
