using System.Collections.Generic;
using Biobanks.Submissions.Models;

namespace Biobanks.Directory.Models.Submissions
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
