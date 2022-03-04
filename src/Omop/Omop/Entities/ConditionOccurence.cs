using System;

namespace Biobanks.Omop.Entities
{
    /// <inheritdoc />
      [Table("condition_occurrence")]
    public class ConditionOccurence
    {
        [Column("condition_occurrence_id")]
        public int ConditionOccurenceId { get; set; }

        [Column("person_id")]
        public int PersonId { get; set; }

        [Column("condition_concept_id")]
        public int ConditionConceptId { get; set; }

        [Column("condition_start_date")]
        public DateTime ConditionStartDate { get; set; }
        
        [Column("condition_start_datetime")]
        public DateTime ConditionStartDatetime { get; set; }
        
        [Column("condition_end_date")]
        public DateTime ConditionEndDate { get; set; }
        
        [Column("condition_end_datetime")]
        public DateTime ConditionEndDatetime { get; set; }
        
        [Column("condition_type_concept_id")]
        public int ConditionTypeConceptId { get; set; }
        
        [Column("condition_status_concept_id")]
        public int ConditionStatusConceptId { get; set; }
        
        [Column("provider_id")]
        public string StopReason { get; set; }
        
        [Column("visit_occurrence_id")]
        public int ProviderId { get; set; }
        
        [Column("visit_occurrence_id")]
        public int VisitOccurenceId { get; set; }
        
        [Column("visit_detail_id")]
        public int VisitDetailId { get; set; }

            
        [Column("condition_source_value")]
        /// <summary>
        /// Represents the condition that occured. E.g. ICD10
        /// </summary>
        public string ConditionSourceValue { get; set; }

            
        [Column("condition_source_concept_id")]
        /// <summary>
        /// Concept that represents the cindition source value
        /// </summary>
        public int ConditionSourceConceptId { get; set; }

            
        [Column("condition_status_source_value")]
        /// <summary>
        /// Represents the conidtion status
        /// </summary>
        public string ConditionStatusSourceValue { get; set; }
    }
}