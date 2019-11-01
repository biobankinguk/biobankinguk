using AutoMapper;
using Biobanks.SubmissionApi.Models;
using Common.Data.Upload;
using SampleModel = Biobanks.Common.Models.SampleModel;

namespace Upload.Profiles
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
