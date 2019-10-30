using AutoMapper;
using Biobanks.SubmissionApi.Models;
using Common.Data.ReferenceData;
using Upload.Common.Data.Entities;
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
