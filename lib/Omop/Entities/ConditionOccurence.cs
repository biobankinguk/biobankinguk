using System;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Omop.Entities
{
    /// <inheritdoc />
    public class ConditionOccurence
    {
        [Key]
        public int ConditionOccurenceId { get; set; }
        public int PersonId { get; set; }
        public int ConditionConceptId { get; set; }
        public DateTime ConditionStartDate { get; set; }
        public DateTime ConditionStartDatetime { get; set; }
        public DateTime ConditionEndDate { get; set; }
        public DateTime ConditionEndDatetime { get; set; }
        public int ConditionTypeConceptId { get; set; }
        public int ConditionStatusConceptId { get; set; }
        public string StopReason { get; set; }
        public int ProviderId { get; set; }
        public int VisitOccurenceId { get; set; }
        public int VisitDetailId { get; set; }
        /// <summary>
        /// Represents the condition that occured. E.g. ICD10
        /// </summary>
        public string ConditionSourceValue { get; set; }
        /// <summary>
        /// Concept that represents the cindition source value
        /// </summary>
        public int ConditionSourceConceptId { get; set; }
        /// <summary>
        /// Represents the conidtion status
        /// </summary>
        public string ConditionStatusSourceValue { get; set; }
    }
}