using System.Collections.Generic;
using Upload.Common.DTO;

namespace Upload.DTOs
{
    /// <inheritdoc />
    public class PaginatedErrorsDto : BasePaginatedDto
    {
        /// <summary>
        /// Collection of error models for the given paginated response DTO.
        /// </summary>
        public ICollection<ErrorDto> Errors { get; set; } = null!;
    }
}
