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
            builder.Services.AddTransient<NewExerciseViewModel>();
            builder.Services.AddTransient<SelectMusclesViewModel>();

            // Pages
            builder.Services.AddTransient<NewExercisePage>();
            builder.Services.AddTransient<SelectMusclesPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
