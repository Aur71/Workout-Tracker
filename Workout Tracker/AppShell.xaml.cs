using Workout_Tracker.View;

namespace Workout_Tracker;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(AddExercisesPage), typeof(AddExercisesPage));
        Routing.RegisterRoute(nameof(NewExercisePage), typeof(NewExercisePage));
        Routing.RegisterRoute(nameof(ExerciseDetailPage), typeof(ExerciseDetailPage));
        Routing.RegisterRoute(nameof(SelectMusclesPage), typeof(SelectMusclesPage));
        Routing.RegisterRoute(nameof(NewProgramPage), typeof(NewProgramPage));
        Routing.RegisterRoute(nameof(ProgramDetailPage), typeof(ProgramDetailPage));
        Routing.RegisterRoute(nameof(NewSessionPage), typeof(NewSessionPage));
        Routing.RegisterRoute(nameof(NewLogPage), typeof(NewLogPage));
        Routing.RegisterRoute(nameof(ActiveWorkoutPage), typeof(ActiveWorkoutPage));
        Routing.RegisterRoute(nameof(EditProgramSchedulePage), typeof(EditProgramSchedulePage));
        Routing.RegisterRoute(nameof(ProgressiveOverloadPage), typeof(ProgressiveOverloadPage));
    }
}
