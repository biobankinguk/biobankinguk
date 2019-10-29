using AutoMapper;
using Biobanks.Common.Data.Entities.ReferenceData;
using Biobanks.SubmissionApi.Models;
using Upload.ViewModels;

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
