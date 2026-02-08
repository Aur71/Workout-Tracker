using Microsoft.Extensions.Logging;
using Workout_Tracker.Services;
using Workout_Tracker.View;
using Workout_Tracker.ViewModel;

namespace Workout_Tracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services
            builder.Services.AddSingleton<DatabaseService>();

            // ViewModels
            builder.Services.AddTransient<ExerciseListViewModel>();
            builder.Services.AddTransient<ExerciseDetailViewModel>();
            builder.Services.AddTransient<NewExerciseViewModel>();
            builder.Services.AddTransient<SelectMusclesViewModel>();
            builder.Services.AddTransient<WorkoutListViewModel>();
            builder.Services.AddTransient<WorkoutDetailViewModel>();
            builder.Services.AddTransient<NewWorkoutViewModel>();
            builder.Services.AddTransient<AddExercisesViewModel>();
            builder.Services.AddTransient<NewProgramViewModel>();
            builder.Services.AddTransient<ProgramListViewModel>();
            builder.Services.AddTransient<ProgramDetailViewModel>();
            builder.Services.AddTransient<NewSessionViewModel>();

            // Pages
            builder.Services.AddTransient<ExerciseListPage>();
            builder.Services.AddTransient<ExerciseDetailPage>();
            builder.Services.AddTransient<NewExercisePage>();
            builder.Services.AddTransient<SelectMusclesPage>();
            builder.Services.AddTransient<WorkoutListPage>();
            builder.Services.AddTransient<WorkoutDetailPage>();
            builder.Services.AddTransient<NewWorkoutPage>();
            builder.Services.AddTransient<AddExercisesPage>();
            builder.Services.AddTransient<NewProgramPage>();
            builder.Services.AddTransient<ProgramListPage>();
            builder.Services.AddTransient<ProgramDetailPage>();
            builder.Services.AddTransient<NewSessionPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
