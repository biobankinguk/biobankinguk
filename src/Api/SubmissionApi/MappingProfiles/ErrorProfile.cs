using AutoMapper;
using Biobanks.Entities.Api;
using Biobanks.SubmissionApi.Models;
using Newtonsoft.Json.Linq;

namespace Biobanks.SubmissionApi.MappingProfiles
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
                            src => JObject.Parse(src.RecordIdentifiers)
                        )
                    );

            CreateMap<ErrorModel, Error>()
                .ForMember(
                    dest => dest.RecordIdentifiers,
                    opts => opts.MapFrom(
                            src => src.RecordIdentifiers.ToString()
                        )
                    );
        }
    }
}
