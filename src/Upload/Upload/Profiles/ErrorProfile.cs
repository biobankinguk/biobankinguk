using AutoMapper;
using Common.Data.Upload;
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
