using AutoMapper;
using Biobanks.Common.Data.Entities;
using Biobanks.SubmissionApi.Models;

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
