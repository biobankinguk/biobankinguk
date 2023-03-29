using AutoMapper;
using Biobanks.Submissions.Api.Models;
using Core.Submissions.Models;

namespace Biobanks.Submissions.Api.MappingProfiles
{
    /// <inheritdoc />
    public class SampleProfile : Profile
    {
        /// <inheritdoc />
        public SampleProfile()
        {
            CreateMap<SampleSubmissionModel, SampleModel>();
            CreateMap<SampleOperationModel, SampleModel>();
        }
    }
}
