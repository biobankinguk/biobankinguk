using System.Collections.Generic;
using Upload.Common.Models;

namespace Upload.DTOs
{
    /// <inheritdoc />
    public class PaginatedErrorsDto : BasePaginatedModel
    {
        /// <summary>
        /// Collection of error models for the given paginated response DTO.
        /// </summary>
        public ICollection<ErrorDto> Errors { get; set; } = null!;
    }
}
