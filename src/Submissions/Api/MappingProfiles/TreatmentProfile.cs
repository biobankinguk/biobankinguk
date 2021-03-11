using AutoMapper;
using Biobanks.Entities.Api;
using Biobanks.Submissions.Api.Models;
using TreatmentModel = Biobanks.Submissions.Core.Models.TreatmentModel;

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

            // base entities
            CreateMap<StagedTreatment, LiveTreatment>();
        }
    }
}
