using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workout_Tracker.Model;
using Workout_Tracker.Services;
using Workout_Tracker.View;

namespace Workout_Tracker.ViewModel;

public partial class CalendarViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private DateTime _currentMonth;
    private CalendarDay? _selectedDay;

    public CalendarViewModel(DatabaseService db)
    {
        _db = db;
        _currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    }

    public ObservableCollection<CalendarDay> CalendarDays { get; } = [];
    public ObservableCollection<CalendarSessionIndicator> SelectedDaySessions { get; } = [];

    [ObservableProperty]
    private string _monthYearDisplay = string.Empty;

    [ObservableProperty]
    private string _selectedDayLabel = string.Empty;

    [ObservableProperty]
    private bool _hasSelectedDaySessions;

    [ObservableProperty]
    private bool _hasDaySelected;

    public async Task LoadMonthAsync()
    {
        var indicators = await _db.GetAllSessionsForMonthAsync(_currentMonth.Year, _currentMonth.Month);
        var grouped = indicators.GroupBy(i => i.Date.Date).ToDictionary(g => g.Key, g => g.ToList());

        MonthYearDisplay = _currentMonth.ToString("MMMM yyyy");
        BuildCalendarGrid(grouped);

        // Auto-select today if in current month, otherwise first of month
        var today = DateTime.Today;
        CalendarDay? autoSelect = null;
        if (today.Year == _currentMonth.Year && today.Month == _currentMonth.Month)
            autoSelect = CalendarDays.FirstOrDefault(d => d.IsToday);
        autoSelect ??= CalendarDays.FirstOrDefault(d => d.IsCurrentMonth);

        if (autoSelect != null)
            SelectDayCellCommand.Execute(autoSelect);
    }

    private void BuildCalendarGrid(Dictionary<DateTime, List<CalendarSessionIndicator>> sessionsByDate)
    {
        CalendarDays.Clear();

        var firstOfMonth = _currentMonth;
        var daysInMonth = DateTime.DaysInMonth(firstOfMonth.Year, firstOfMonth.Month);

        // Sunday = 0 start
        int startDow = (int)firstOfMonth.DayOfWeek;

        // Leading days from previous month
        var prevMonth = firstOfMonth.AddMonths(-1);
        int prevDays = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
        for (int i = startDow - 1; i >= 0; i--)
        {
            var date = new DateTime(prevMonth.Year, prevMonth.Month, prevDays - i);
            CalendarDays.Add(new CalendarDay
            {
                Date = date,
                IsCurrentMonth = false,
                IsToday = date.Date == DateTime.Today,
                Sessions = sessionsByDate.GetValueOrDefault(date.Date, [])
            });
        }

        // Current month days
        for (int d = 1; d <= daysInMonth; d++)
        {
            var date = new DateTime(firstOfMonth.Year, firstOfMonth.Month, d);
            CalendarDays.Add(new CalendarDay
            {
                Date = date,
                IsCurrentMonth = true,
                IsToday = date.Date == DateTime.Today,
                Sessions = sessionsByDate.GetValueOrDefault(date.Date, [])
            });
        }

        // Trailing days to fill 42 cells
        int trailing = 42 - CalendarDays.Count;
        var nextMonth = firstOfMonth.AddMonths(1);
        for (int d = 1; d <= trailing; d++)
        {
            var date = new DateTime(nextMonth.Year, nextMonth.Month, d);
            CalendarDays.Add(new CalendarDay
            {
                Date = date,
                IsCurrentMonth = false,
                IsToday = date.Date == DateTime.Today,
                Sessions = sessionsByDate.GetValueOrDefault(date.Date, [])
            });
        }
    }

    [RelayCommand]
    private void SelectDayCell(CalendarDay day)
    {
        if (_selectedDay != null)
            _selectedDay.IsSelected = false;

        day.IsSelected = true;
        _selectedDay = day;

        SelectedDayLabel = day.Date.ToString("dddd, MMMM d");
        HasDaySelected = true;

        SelectedDaySessions.Clear();
        foreach (var s in day.Sessions)
            SelectedDaySessions.Add(s);
        HasSelectedDaySessions = SelectedDaySessions.Count > 0;
    }

    [RelayCommand]
    private async Task PreviousMonth()
    {
        _currentMonth = _currentMonth.AddMonths(-1);
        _selectedDay = null;
        await LoadMonthAsync();
    }

    [RelayCommand]
    private async Task NextMonth()
    {
        _currentMonth = _currentMonth.AddMonths(1);
        _selectedDay = null;
        await LoadMonthAsync();
    }

    [RelayCommand]
    private async Task GoToToday()
    {
        _currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        _selectedDay = null;
        await LoadMonthAsync();
    }

    [RelayCommand]
    private async Task OpenSession(CalendarSessionIndicator indicator)
    {
        if (indicator.ProgramId.HasValue)
            await Shell.Current.GoToAsync($"{nameof(ProgramDetailPage)}?id={indicator.ProgramId.Value}");
    }
}
