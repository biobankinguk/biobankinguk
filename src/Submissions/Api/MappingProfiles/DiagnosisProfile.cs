using AutoMapper;
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
            CreateMap<DiagnosisOperationModel, DiagnosisModel>();
        }
    }
}
