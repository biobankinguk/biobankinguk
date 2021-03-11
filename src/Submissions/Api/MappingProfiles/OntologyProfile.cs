using AutoMapper;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.SubmissionApi.Models;

namespace Biobanks.SubmissionApi.MappingProfiles
{
    /// <inheritdoc />
    public class OntologyProfile : Profile
    {
        /// <inheritdoc />
        public OntologyProfile()
        {
            CreateMap<Ontology, OntologyModel>();
            CreateMap<OntologyVersion, OntologyVersionModel>();
        }
    }
}
