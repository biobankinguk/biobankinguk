using AutoMapper;
using Biobanks.Entities.Api;
using Core.Submissions.Dto;
using Core.Submissions.Models;

namespace Core.Submissions.MappingProfiles
{
    public class DiagnosisProfile : Profile
    {
        public DiagnosisProfile()
        {
            CreateMap<DiagnosisIdModel, DiagnosisModel>();

            CreateMap<DiagnosisModel, DiagnosisDto>();
            CreateMap<DiagnosisDto, DiagnosisModel>();

            CreateMap<StagedDiagnosis, DiagnosisModel>();
            CreateMap<StagedDiagnosis, LiveDiagnosis>();
        }
    }
}
