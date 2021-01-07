using AutoMapper;
using Entities.Api;
using Biobanks.Common.Models;
using Biobanks.SubmissionAzureFunction.Dtos;

namespace Biobanks.SubmissionAzureFunction.MappingProfiles
{
    public class SampleProfile : Profile
    {
        public SampleProfile()
        {
            CreateMap<StagedSample, LiveSample>();
            CreateMap<SampleModel, SampleDto>();
            CreateMap<SampleIdModel, SampleIdDto>();
        }
    }
}
