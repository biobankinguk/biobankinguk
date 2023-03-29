using Biobanks.Entities.Data.ReferenceData;
using System;
using System.Xml;

namespace Biobanks.Submissions.Extensions
{
    public static class AgeRangeExtension
    {
        public static bool ContainsTimeSpan(this AgeRange ageRange, TimeSpan time)
        {
            var noLower = string.IsNullOrEmpty(ageRange.LowerBound);
            var noUpper = string.IsNullOrEmpty(ageRange.UpperBound);

            if (noLower && noUpper)
                return true;

            if (noUpper)
                return XmlConvert.ToTimeSpan(ageRange.LowerBound) <= time;

            if (noLower)
                return XmlConvert.ToTimeSpan(ageRange.UpperBound) >= time;

            return XmlConvert.ToTimeSpan(ageRange.LowerBound) <= time
                && XmlConvert.ToTimeSpan(ageRange.UpperBound) >= time;
        }

        public static bool ContainsTimeSpan(this AgeRange ageRange, string time)
            => !string.IsNullOrEmpty(time) && ContainsTimeSpan(ageRange, XmlConvert.ToTimeSpan(time));
    }
}
