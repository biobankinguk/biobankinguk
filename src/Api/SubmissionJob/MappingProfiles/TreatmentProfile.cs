using AutoMapper;
using Biobanks.Common.Data.Entities;
using Biobanks.Common.Models;
using Biobanks.SubmissionJob.Dtos;

namespace Biobanks.SubmissionJob.MappingProfiles
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
