using AutoMapper;
using Entities.Api;
using Biobanks.SubmissionApi.Models;
using TreatmentModel = Biobanks.Common.Models.TreatmentModel;

namespace Biobanks.SubmissionApi.MappingProfiles
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
