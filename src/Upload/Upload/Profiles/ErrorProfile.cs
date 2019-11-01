using AutoMapper;
using Biobanks.SubmissionApi.Models;
using Common.Data.Upload;

namespace Upload.Profiles
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
