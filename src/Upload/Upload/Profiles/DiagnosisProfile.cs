using AutoMapper;
using Biobanks.Common.Models;
using Biobanks.SubmissionApi.Models;
using Common.Data.Upload;

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
