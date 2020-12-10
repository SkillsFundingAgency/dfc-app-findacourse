using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DFC.App.FindACourse.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class DateTimeExtensions
    {
        public static string ToFormattedString(this DateTime value)
        {
            string daySuffix = (value.Day % 100) switch
            {
                11 => "th",
                12 => "th",
                13 => "th",
                _ => (value.Day % 10) switch
                {
                    1 => "st",
                    2 => "nd",
                    3 => "rd",
                    _ => "th",
                }
            };

            var formattedValue = value.ToString("d?? MMMM yyyy", CultureInfo.InvariantCulture).Replace("??", daySuffix, StringComparison.OrdinalIgnoreCase);

            return formattedValue;
        }
    }
}
