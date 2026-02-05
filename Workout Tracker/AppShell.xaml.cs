using Workout_Tracker.View;

namespace Workout_Tracker;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(NewWorkoutPage), typeof(NewWorkoutPage));
        Routing.RegisterRoute(nameof(AddExercisesPage), typeof(AddExercisesPage));
        Routing.RegisterRoute(nameof(WorkoutDetailPage), typeof(WorkoutDetailPage));
        Routing.RegisterRoute(nameof(NewExercisePage), typeof(NewExercisePage));
        Routing.RegisterRoute(nameof(ExerciseDetailPage), typeof(ExerciseDetailPage));
        Routing.RegisterRoute(nameof(SelectMusclesPage), typeof(SelectMusclesPage));
    }
}
