using System.Text.Json;
using AutoMapper;
using Biobanks.Data.Entities.Api;
using Biobanks.Directory.Models.Submissions;

namespace Biobanks.Directory.MappingProfiles
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
