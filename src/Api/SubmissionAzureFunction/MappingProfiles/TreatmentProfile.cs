using AutoMapper;
using Entities.Api;
using Biobanks.Common.Models;
using Biobanks.SubmissionAzureFunction.Dtos;

namespace Biobanks.SubmissionAzureFunction.MappingProfiles
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
