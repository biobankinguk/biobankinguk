using AutoMapper;
using Common.Data.ReferenceData;
using Upload.Common.Data.Entities;
using Upload.DTO;

namespace Upload.Profiles
{
    /// <inheritdoc />
    public class OntologyProfile : Profile
    {
        /// <inheritdoc />
        public OntologyProfile()
        {
            CreateMap<Ontology, OntologyDto>();
            CreateMap<OntologyVersion, OntologyVersionDto>();
        }
    }
}
