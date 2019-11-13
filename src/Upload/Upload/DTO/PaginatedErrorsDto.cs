using System.Collections.Generic;
using Upload.Common.DTO;
using Upload.DTO;

namespace Upload.DTO
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
