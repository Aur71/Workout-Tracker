using System.Globalization;

namespace Workout_Tracker.Converters;

public class FilterChipBgConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var selected = value as string;
        var chip = parameter as string;
        bool isActive = string.Equals(selected, chip, StringComparison.OrdinalIgnoreCase);

        if (isActive)
            return Application.Current!.Resources["Primary"];

        return Application.Current!.RequestedTheme == AppTheme.Dark
            ? Application.Current.Resources["CardBackground"]
            : Application.Current.Resources["White"];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class FilterChipTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var selected = value as string;
        var chip = parameter as string;
        bool isActive = string.Equals(selected, chip, StringComparison.OrdinalIgnoreCase);

        if (isActive)
            return Application.Current!.Resources["Black"];

        return Application.Current!.RequestedTheme == AppTheme.Dark
            ? Application.Current.Resources["Gray400"]
            : Application.Current.Resources["Gray600"];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
