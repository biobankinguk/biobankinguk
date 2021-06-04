using AutoMapper;
using Biobanks.Entities.Api;
using Core.Submissions.Dto;
using Core.Submissions.Models;

namespace Core.Submissions.MappingProfiles
{
    public class SampleProfile : Profile
    {
        public SampleProfile()
        {
            CreateMap<SampleModel, SampleDto>();
            CreateMap<SampleIdModel, SampleIdDto>();

            CreateMap<StagedSample, LiveSample>();
        }
    }
}
