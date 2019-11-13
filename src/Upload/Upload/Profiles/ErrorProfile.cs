using AutoMapper;
using Upload.Common.Data.Entities;
using Upload.DTOs;

namespace Upload.Profiles
{
    /// <inheritdoc />
    public class ErrorProfile : Profile
    {
        /// <inheritdoc />
        public ErrorProfile()
        {
            CreateMap<Error, ErrorDto>();
        }
    }
}
