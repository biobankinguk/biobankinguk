using AutoMapper;
using Upload.SubmissionApi.Models;
using Common.Data.Upload;
using Upload.DTOs;
using SampleModel = Upload.Common.Models.SampleModel;

namespace Upload.Profiles
{
    /// <inheritdoc />
    public class SampleProfile : Profile
    {
        /// <inheritdoc />
        public SampleProfile()
        {
            CreateMap<SampleSubmissionModel, SampleModel>();
            CreateMap<SampleOperationDto, SampleModel>();

            // base entities
            CreateMap<StagedSample, LiveSample>();
        }
    }
}
