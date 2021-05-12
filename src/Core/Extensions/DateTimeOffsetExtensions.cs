using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Biobanks.Extensions
{

    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Determine if a DateTimeOffset is between two other DateTimeOffsets
        /// </summary>
        /// <param name="date"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public static bool IsBetween(this DateTimeOffset date, DateTimeOffset startDate, DateTimeOffset endDate)
            => date >= startDate && date <= endDate;

        /// <summary>
        /// Get which Quarter of the Calendar year we are in, numerically.
        /// </summary>
        /// <param name="date"></param>
        public static int GetQuarter(this DateTimeOffset date)
            => (date.Month + 2) / 3;

        /// <summary>
        /// Convert the DateTimeOffset to a custom string,
        /// containing the Quarter of the Calendar Year,
        /// or any other valid formatting of the DateTimeOffset
        /// interpolated as many times as specified.
        /// </summary>
        /// <param name="date">The DateTimeOffset</param>
        /// <param name="format">The string to interpolate formatted DateTimeOffset values into.
        /// Curly braces denote insert points. The brace content must be either "Q" to insert the Quarter,
        /// or a valid DateTimeOffset format string to insert that result.</param>
        /// <returns></returns>
        public static string ToQuarterString(this DateTimeOffset date, string format = "{yyyy}Q{Q}")
            => new Regex("{([^}]+)}").Replace(format,
                match => {
                    // 1st group is the whole match; 2nd group is the first capture group :)
                    var formatString = match.Groups[1].Captures[0].Value;

                    return formatString == "Q"
                        ? date.GetQuarter().ToString()
                        : date.ToString(formatString);
                });
    }
}
