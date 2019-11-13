using AutoMapper;
using Upload.Common.Data.Entities;
using Upload.DTOs;
using SampleDto = Upload.Common.Models.SampleDto;

namespace Upload.Profiles
{
    /// <inheritdoc />
    public class SampleProfile : Profile
    {
        /// <inheritdoc />
        public SampleProfile()
        {
            CreateMap<SampleSubmissionDto, SampleDto>();
            CreateMap<SampleOperationDto, SampleDto>();

            // base entities
            CreateMap<StagedSample, LiveSample>();
        }
    }
}
