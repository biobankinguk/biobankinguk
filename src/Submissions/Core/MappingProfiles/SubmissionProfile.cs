using System;
using System.Linq;
using AutoMapper;
using Biobanks.Entities.Api;
using Biobanks.Submissions.Core.Models;

namespace Biobanks.Submissions.Core.MappingProfiles
{
    /// <inheritdoc />
    public class SubmissionProfile : Profile
    {
        /// <inheritdoc />
        public SubmissionProfile()
        {
            CreateMap<Submission, SubmissionSummaryModel>()
                .ForMember(
                    dest => dest.Status,
                    opts => opts.MapFrom(
                        src => src.Status.Value))
                .ForMember(
                    dest => dest.ErrorCount,
                    opts => opts.MapFrom(
                        src => src.Errors.Count))
                .ForMember(
                    dest => dest.RecordsFailed,
                    opts => opts.MapFrom((src, _) =>
                        src.Errors?.Select(e => e.RecordIdentifiers).Distinct().Count()))
                .ForMember(
                    dest => dest.ErrorUri,
                    opts => opts.MapFrom(
                        src => new Uri(
                            $"/status/{src.Id}/error/",
                            UriKind.Relative)));
        }
    }
}
