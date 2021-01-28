using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

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

        public static bool IsPostcode(this string value)
        {
            value = value.Replace(" ", string.Empty);

            var postcodeRegex = new Regex(@"^([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})");

            if (postcodeRegex.IsMatch(value.ToUpper()))
            {
                return true;
            }

            return false;
        }
    }
}
