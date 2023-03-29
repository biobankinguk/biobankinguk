using AutoMapper;
using Biobanks.Entities.Api;
using Core.Submissions.Dto;
using Core.Submissions.Models;

namespace Core.Submissions.MappingProfiles
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
