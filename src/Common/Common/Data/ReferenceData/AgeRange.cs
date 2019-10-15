using System.ComponentModel.DataAnnotations;

namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Reference Data. Age range with a maximum and minmum value for a given sample.
    /// </summary>
    public class AgeRange : SortedBaseReferenceDatum
    {
        public AgeRange(string value) : base(value)
        {
        }
    }
}
