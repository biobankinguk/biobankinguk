using AutoMapper;
using Biobanks.Entities.Api;
using Biobanks.Submissions.Core.Models;
using Biobanks.Submissions.Core.Dto;

namespace Biobanks.Submissions.Core.MappingProfiles
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
