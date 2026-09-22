using System.Globalization;

namespace POMuswick.Parsers
{
    public class BaseParser
    {
        public DateTime GetDateTime(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DateTime.MinValue;

            value = value.Trim();

            // First try exact formats
            string[] formats =
            {
                "M/d/yyyy",
                "MM/dd/yyyy",
                "yyyy-MM-dd",
                "yyyyMMdd",
                "M/d/yy",
                "MM/dd/yy"
            };

            if (DateTime.TryParseExact(
                    value,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date))
            {
                return date;
            }

            // Fallback to normal parsing
            if (DateTime.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date))
            {
                return date;
            }

            Console.WriteLine($"{key} Invalid Date: '{value}'");

            return DateTime.MinValue;
        }
        public int GetIntegerValue(String key, String value, int defaultValue)
        {
            try
            {
                string sizeValue = value.Trim();
                if (sizeValue.Length > 0)
                {
                    string digits = new string(sizeValue
                    .TakeWhile(char.IsDigit)
                    .ToArray());

                    return int.TryParse(digits, out var size)
                        ? size
                        : defaultValue;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(key + "Converting string to int" + e.Message);
                return defaultValue;
            }
        }

        public Decimal GetDecimalValue(String key, String value, Decimal defaultValue)
        {
            try
            {
                string sizeValue = value.Trim();
                if (sizeValue.Length != 0)
                    return Convert.ToDecimal(sizeValue);
                else
                    return defaultValue;
            }
            catch (Exception e)
            {
                Console.WriteLine(key + "Converting string to Decimal " + e.Message);
                return defaultValue;
            }
        }
    }
}