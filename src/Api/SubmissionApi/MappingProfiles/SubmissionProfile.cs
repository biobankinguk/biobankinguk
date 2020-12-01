using System;
using System.Linq;
using AutoMapper;
using Biobanks.Common.Data.Entities;
using Biobanks.Common.Models;

namespace Biobanks.SubmissionApi.MappingProfiles
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
                    opts => opts.ResolveUsing(
                        src => src.Errors?.Select(e => e.RecordIdentifiers).Distinct().Count()))
                .ForMember(
                    dest => dest.ErrorUri,
                    opts => opts.MapFrom(
                        src => new Uri(
                            $"/status/{src.Id}/error/",
                            UriKind.Relative)));
        }
    }
}
