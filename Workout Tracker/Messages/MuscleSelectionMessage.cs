using CommunityToolkit.Mvvm.Messaging.Messages;
using Workout_Tracker.Model;

namespace Workout_Tracker.Messages;

public class MuscleSelectionMessage : ValueChangedMessage<List<MuscleSelection>>
{
    public MuscleSelectionMessage(List<MuscleSelection> value) : base(value) { }
}
