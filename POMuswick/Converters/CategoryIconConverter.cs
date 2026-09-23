using System.Globalization;

namespace POMuswick.Converters;

public sealed class CategoryIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string title = value?.ToString()?.ToUpperInvariant() ?? string.Empty;

        if (title.Contains("BEER") || title.Contains("WINE") || title.Contains("DRINK") || title.Contains("BEVERAGE"))
            return "\uf0fc";

        if (title.Contains("TOBACCO") || title.Contains("CIGAR") || title.Contains("SMOK"))
            return "\uf54e";

        if (title.Contains("CLEAN") || title.Contains("PAPER") || title.Contains("HOUSE"))
            return "\uf2ed";

        if (title.Contains("FOOD") || title.Contains("SNACK") || title.Contains("GROC"))
            return "\uf2e7";

        return "\uf291";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}