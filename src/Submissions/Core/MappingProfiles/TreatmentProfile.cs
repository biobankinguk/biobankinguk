using AutoMapper;
using Biobanks.Entities.Api;
using Biobanks.Submissions.Core.Models;
using Biobanks.Submissions.Core.Dto;

namespace Biobanks.Submissions.Core.MappingProfiles
{
    public class TreatmentProfile : Profile
    {
        public TreatmentProfile()
        {            
            CreateMap<TreatmentModel, TreatmentDto>();
            CreateMap<TreatmentDto, TreatmentModel>();

            CreateMap<StagedTreatment, TreatmentModel>();
            CreateMap<StagedTreatment, LiveTreatment>();
        }
    }
}
