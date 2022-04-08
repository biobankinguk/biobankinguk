using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared
{
    public class AccessConditionModel
    {   /// <summary> 
        /// Id
        ///</summary>
        ///<example>3</example>
        [Required]
        public int Id { get; set; }
        /// <summary> 
        /// Description
        ///</summary>
        ///<example>Open to applicants</example>
        [Required]
        public string Description { get; set; }
        /// <summary> 
        /// Sort Order
        ///</summary>
        ///<example>1</example>
        [Required]
        public int SortOrder { get; set; }

    }
}
