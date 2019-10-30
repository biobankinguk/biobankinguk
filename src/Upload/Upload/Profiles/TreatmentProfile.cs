using AutoMapper;
using Biobanks.Common.Models;
using Biobanks.SubmissionApi.Models;
using Common.Data.Upload;

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
