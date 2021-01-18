using AutoMapper;
using Entities.Api;
using Biobanks.SubmissionApi.Models;
using SampleModel = Biobanks.Common.Models.SampleModel;

namespace Biobanks.SubmissionApi.MappingProfiles
{
    /// <inheritdoc />
    public class SampleProfile : Profile
    {
        /// <inheritdoc />
        public SampleProfile()
        {
            CreateMap<SampleSubmissionModel, SampleModel>();
            CreateMap<SampleOperationModel, SampleModel>();

            // base entities
            CreateMap<StagedSample, LiveSample>();
        }
    }
}
