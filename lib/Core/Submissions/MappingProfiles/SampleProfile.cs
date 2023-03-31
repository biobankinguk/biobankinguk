using AutoMapper;
using Biobanks.Data.Entities.Api;
using Biobanks.Submissions.Dto;
using Biobanks.Submissions.Models;

namespace Biobanks.Submissions.MappingProfiles
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
