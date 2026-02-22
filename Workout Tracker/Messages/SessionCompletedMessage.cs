using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Workout_Tracker.Messages;

public class SessionCompletedMessage : ValueChangedMessage<int>
{
    public SessionCompletedMessage(int sessionId) : base(sessionId) { }
}
