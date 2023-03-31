using AutoMapper;
using Biobanks.Data.Entities.Api;
using Biobanks.Submissions.Dto;
using Biobanks.Submissions.Models;

namespace Biobanks.Submissions.MappingProfiles
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
