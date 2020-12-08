using AutoMapper;
using Biobanks.Common.Data.Entities;
using Biobanks.Common.Models;
using Biobanks.SubmissionAzureFunction.Dtos;

namespace Biobanks.SubmissionAzureFunction.MappingProfiles
{
    public class DiagnosisProfile : Profile
    {
        public DiagnosisProfile()
        {
            CreateMap<DiagnosisModel, DiagnosisDto>();
            CreateMap<DiagnosisDto, DiagnosisModel>();
            CreateMap<StagedDiagnosis, DiagnosisModel>();
            CreateMap<StagedDiagnosis, LiveDiagnosis>();
        }
    }
}
