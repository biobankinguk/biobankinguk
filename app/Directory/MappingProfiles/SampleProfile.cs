using AutoMapper;
using Biobanks.Directory.Models.Submissions;
using Biobanks.Submissions.Models;

namespace Biobanks.Directory.MappingProfiles
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
