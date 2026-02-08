using CommunityToolkit.Mvvm.ComponentModel;

namespace Workout_Tracker.Model;

public partial class CalendarDay : ObservableObject
{
    public DateTime Date { get; set; }
    public bool IsCurrentMonth { get; set; }
    public bool IsToday { get; set; }
    public bool IsPast => Date.Date < DateTime.Today;

    [ObservableProperty]
    private bool _isSelected;

    public List<CalendarSessionIndicator> Sessions { get; set; } = [];

    public int DayNumber => Date.Day;
    public bool HasSessions => Sessions.Count > 0;
    public List<CalendarSessionIndicator> VisibleDots => Sessions.Take(3).ToList();
}
