using AutoMapper;
using Biobanks.Common.Data.Entities;
using Biobanks.Common.Models;
using Biobanks.SubmissionStagingJob.Dtos;

namespace Biobanks.SubmissionStagingJob.MappingProfiles
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
