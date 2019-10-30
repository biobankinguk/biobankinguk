using AutoMapper;
using Biobanks.SubmissionApi.Models;
using Common.Data.Upload;

namespace Biobanks.SubmissionApi.MappingProfiles
{
    /// <inheritdoc />
    public class ErrorProfile : Profile
    {
        /// <inheritdoc />
        public ErrorProfile()
        {
            CreateMap<Error, ErrorModel>();
        }
    }
}
