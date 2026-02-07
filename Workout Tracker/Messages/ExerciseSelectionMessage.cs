using CommunityToolkit.Mvvm.Messaging.Messages;
using Workout_Tracker.Model;

namespace Workout_Tracker.Messages;

public class ExerciseSelectionMessage : ValueChangedMessage<List<ExerciseSelection>>
{
    public ExerciseSelectionMessage(List<ExerciseSelection> value) : base(value) { }
}
