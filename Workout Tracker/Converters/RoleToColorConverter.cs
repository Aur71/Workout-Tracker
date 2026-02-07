using System.Globalization;

namespace Workout_Tracker.Converters;

/// <summary>
/// Converts a role string ("primary"/"secondary") to a color.
/// Set TargetRole to match, ActiveColor for match, InactiveColor for no match.
/// </summary>
public class RoleToColorConverter : IValueConverter
{
    public string TargetRole { get; set; } = "primary";
    public Color? ActiveColor { get; set; }
    public Color? InactiveColor { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string role && role == TargetRole)
            return ActiveColor;
        return InactiveColor;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
