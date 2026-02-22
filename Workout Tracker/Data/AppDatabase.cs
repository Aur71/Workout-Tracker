using SQLite;
using Workout_Tracker.Model;
using Workout_Tracker.Services;

public class AppDatabase
{
    private static bool _isSeeded = false;

    public static SQLiteAsyncConnection Database
    {
        get
        {
            if (field == null)
            {
                var dbPath = Path.Combine(
                    FileSystem.AppDataDirectory,
                    "workout_tracker.db"
                );

                field = new SQLiteAsyncConnection(dbPath);

                // Creating tables (if they don't exist)
                field.CreateTableAsync<Tag>().Wait();
                field.CreateTableAsync<Program>().Wait();
                field.CreateTableAsync<Exercise>().Wait();
                field.CreateTableAsync<Muscle>().Wait();
                field.CreateTableAsync<ExerciseMuscle>().Wait();
                field.CreateTableAsync<Session>().Wait();
                field.CreateTableAsync<SessionExercise>().Wait();
                field.CreateTableAsync<Set>().Wait();
                field.CreateTableAsync<BodyMetric>().Wait();
                field.CreateTableAsync<RecoveryLog>().Wait();
                field.CreateTableAsync<CalorieLog>().Wait();

                // Seed default data
                if (!_isSeeded)
                {
                    DatabaseSeeder.SeedAsync().Wait();
                    _isSeeded = true;
                }
            }

            return field;
        }

        private set;
    }
}

