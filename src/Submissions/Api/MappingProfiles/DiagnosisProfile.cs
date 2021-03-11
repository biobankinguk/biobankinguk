using AutoMapper;
using Biobanks.Entities.Api;
using Biobanks.Submissions.Core.Models;
using Biobanks.Submissions.Api.Models;

namespace Biobanks.Submissions.Api.MappingProfiles
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
