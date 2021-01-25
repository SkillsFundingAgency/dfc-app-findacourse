using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DFC.App.FindACourse.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class StringExtensions
    {
        public static string ToDecimalString(this string value, string format)
        {
            string formattedValue = "0.00";

            if (!string.IsNullOrWhiteSpace(value) && decimal.TryParse(value, out var decimalValue))
            {
                formattedValue = string.Format(CultureInfo.InvariantCulture, "{0:" + format + "}", decimalValue);
            }

            return formattedValue;
        }
    }
}
