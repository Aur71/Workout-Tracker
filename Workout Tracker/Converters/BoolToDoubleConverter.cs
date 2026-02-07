using System.Globalization;

namespace Workout_Tracker.Converters;

public class BoolToDoubleConverter : IValueConverter
{
    public double TrueValue { get; set; }
    public double FalseValue { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? TrueValue : FalseValue;
        return FalseValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
