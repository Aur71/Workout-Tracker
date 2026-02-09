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
                Workouts = await db.Table<Workout>().ToListAsync(),
                WorkoutExercises = await db.Table<WorkoutExercise>().ToListAsync(),
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

        if (data.Version <= 0 || data.Version > CurrentVersion)
            return (false, "This backup was created by a newer version of the app. Please update the app first.");

        if (data.Data == null)
            return (false, "This backup file is empty or corrupted.");

        return (true, null);
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
        payload.Workouts ??= [];
        payload.WorkoutExercises ??= [];
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
            conn.DeleteAll<WorkoutExercise>();
            conn.DeleteAll<Workout>();
            conn.DeleteAll<ExerciseMuscle>();
            conn.DeleteAll<Exercise>();
            conn.DeleteAll<Muscle>();
            conn.DeleteAll<Tag>();
            conn.DeleteAll<Program>();
            conn.DeleteAll<BodyMetric>();
            conn.DeleteAll<RecoveryLog>();
            conn.DeleteAll<CalorieLog>();

            // Insert all data (parent tables first, preserving original IDs)
            conn.InsertAll(payload.Tags);
            conn.InsertAll(payload.Muscles);
            conn.InsertAll(payload.Exercises);
            conn.InsertAll(payload.ExerciseMuscles);
            conn.InsertAll(payload.Programs);
            conn.InsertAll(payload.Workouts);
            conn.InsertAll(payload.WorkoutExercises);
            conn.InsertAll(payload.Sessions);
            conn.InsertAll(payload.SessionExercises);
            conn.InsertAll(payload.Sets);
            conn.InsertAll(payload.BodyMetrics);
            conn.InsertAll(payload.RecoveryLogs);
            conn.InsertAll(payload.CalorieLogs);
        });
    }
}
