using System.Collections.Generic;
using Biobanks.Common.Models;

namespace Biobanks.SubmissionApi.Models
{
    /// <inheritdoc />
    public class PaginatedErrorsModel : BasePaginatedModel
    {
        /// <summary>
        /// Collection of error models for the given paginated response viewmodel.
        /// </summary>
        public ICollection<ErrorModel> Errors { get; set; }
    }
}
