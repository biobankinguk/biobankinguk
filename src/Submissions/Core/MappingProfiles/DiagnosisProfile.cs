using AutoMapper;
using Biobanks.Entities.Api;
using Biobanks.Submissions.Core.Models;
using Biobanks.Submissions.Core.Dto;

namespace Biobanks.Submissions.Core.MappingProfiles
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
