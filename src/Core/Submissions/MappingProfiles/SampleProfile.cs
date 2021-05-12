using AutoMapper;
using Biobanks.Entities.Api;
using Biobanks.Submissions.Core.Models;
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
