using AutoMapper;
using Biobanks.Submissions.Api.Models;
using Core.Submissions.Models;

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
