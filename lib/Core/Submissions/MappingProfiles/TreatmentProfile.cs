using AutoMapper;
using Biobanks.Data.Entities.Api;
using Biobanks.Submissions.Dto;
using Biobanks.Submissions.Models;

namespace Biobanks.Submissions.MappingProfiles
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
