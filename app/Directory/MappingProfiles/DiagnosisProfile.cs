using AutoMapper;
using Biobanks.Directory.Models.Submissions;
using Biobanks.Submissions.Models;

namespace Biobanks.Directory.MappingProfiles
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
