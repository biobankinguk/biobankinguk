using AutoMapper;
using Biobanks.Submissions.Api.Models;
using Core.Submissions.Models;

namespace Biobanks.Submissions.Api.MappingProfiles
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
