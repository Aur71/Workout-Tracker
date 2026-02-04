using SQLite;
using Workout_Tracker.Model;

public class AppDatabase
{
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
                field.CreateTableAsync<Workout>().Wait();
                field.CreateTableAsync<WorkoutExercise>().Wait();
                field.CreateTableAsync<Session>().Wait();
                field.CreateTableAsync<SessionExercise>().Wait();
                field.CreateTableAsync<Set>().Wait();
                field.CreateTableAsync<BodyMetric>().Wait();
                field.CreateTableAsync<RecoveryLog>().Wait();
                field.CreateTableAsync<CalorieLog>().Wait();
            }

            return field;
        }

        private set;
    }
}