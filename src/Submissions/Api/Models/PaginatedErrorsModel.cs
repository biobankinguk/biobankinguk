using System.Collections.Generic;
using Biobanks.Submissions.Core.Models;

namespace Biobanks.Submissions.Api.Models
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
