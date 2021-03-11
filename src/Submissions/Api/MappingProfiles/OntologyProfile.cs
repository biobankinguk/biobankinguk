using AutoMapper;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Submissions.Api.Models;

namespace Biobanks.Submissions.Api.MappingProfiles
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
