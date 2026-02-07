using System.Globalization;

namespace Workout_Tracker.Converters;

public class RoleToFontAttributeConverter : IValueConverter
{
    public string TargetRole { get; set; } = "primary";

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string role && role == TargetRole)
            return FontAttributes.Bold;
        return FontAttributes.None;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
