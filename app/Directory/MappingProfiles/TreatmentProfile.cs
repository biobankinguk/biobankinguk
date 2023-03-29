using AutoMapper;
using Biobanks.Directory.Models.Submissions;
using Biobanks.Submissions.Models;

namespace Biobanks.Directory.MappingProfiles
{
    /// <inheritdoc />
    public class TreatmentProfile : Profile
    {
        /// <inheritdoc />
        public TreatmentProfile()
        {
            CreateMap<TreatmentSubmissionModel, TreatmentModel>();
            CreateMap<TreatmentOperationModel, TreatmentModel>();
        }
    }
}
