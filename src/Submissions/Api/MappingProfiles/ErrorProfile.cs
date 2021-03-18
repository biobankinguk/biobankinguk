using AutoMapper;

using Biobanks.Entities.Api;
using Biobanks.Submissions.Api.Models;

using System.Text.Json;

namespace Biobanks.Submissions.Api.MappingProfiles
{
    /// <inheritdoc />
    public class ErrorProfile : Profile
    {
        /// <inheritdoc />
        public ErrorProfile()
        {
            CreateMap<Error, ErrorModel>()
                .ForMember(
                    dest => dest.RecordIdentifiers,
                    opts => opts.MapFrom(
                            src => JsonDocument.Parse(src.RecordIdentifiers, default)));

            CreateMap<ErrorModel, Error>()
                .ForMember(
                    dest => dest.RecordIdentifiers,
                    opts => opts.MapFrom(
                            src => src.RecordIdentifiers.ToString()));
        }
    }
}
