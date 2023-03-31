using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biobanks.Data.Entities.Api
{
    /// <summary>
    /// So we can stage record ids to be deleted
    /// </summary>
    public class StagedDelete
    {
        /// <summary>
        /// Internal id of the relevant entity to delete.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// Foreign key field.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int OrganisationId { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    /// Subclass for EF Core.
    /// </summary>
    public class StagedDiagnosisDelete : StagedDelete { }
    /// <inheritdoc />
    /// <summary>
    /// Subclass for EF Core.
    /// </summary>
    public class StagedSampleDelete : StagedDelete { }
    /// <inheritdoc />
    /// <summary>
    /// Subclass for EF Core.
    /// </summary>
    public class StagedTreatmentDelete : StagedDelete { }
}
