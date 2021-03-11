using AutoMapper;
using Biobanks.Submissions.Api.Models;
using SampleModel = Biobanks.Submissions.Core.Models.SampleModel;

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
