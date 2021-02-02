using AutoMapper;
using Biobanks.Entities.Api;
using Biobanks.Common.Models;
using Biobanks.SubmissionApi.Models;

namespace Biobanks.SubmissionApi.MappingProfiles
{
    /// <inheritdoc />
    public class DiagnosisProfile : Profile
    {
        /// <inheritdoc />
        public DiagnosisProfile()
        {
            CreateMap<DiagnosisIdModel, DiagnosisModel>();
            CreateMap<DiagnosisOperationModel, DiagnosisModel>();

            // base entities
            CreateMap<StagedDiagnosis, LiveDiagnosis>();
        }
    }
}
