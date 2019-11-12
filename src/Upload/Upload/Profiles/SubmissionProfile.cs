using System;
using System.Linq;
using AutoMapper;
using Upload.Common.Models;
using Common.Data.Upload;

namespace Upload.Profiles
{
    /// <inheritdoc />
    public class SubmissionProfile : Profile
    {
        /// <inheritdoc />
        public SubmissionProfile()
        {
            CreateMap<Submission, SubmissionSummaryDto>()
                .ForMember(
                    dest => dest.UploadStatus,
                    opts => opts.MapFrom(
                        src => src.UploadStatus))
                .ForMember(
                    dest => dest.ErrorCount,
                    opts => opts.MapFrom(
                        src => src.Errors.Count))
                .ForMember(
                    dest => dest.RecordsFailed,
                    opts => opts.MapFrom(src => src.Errors.Select(e => e.RecordIdentifiers).Distinct().Count()))
                .ForMember(
                    dest => dest.ErrorUri,
                    opts => opts.MapFrom(
                        src => new Uri(
                            $"/status/{src.Id}/error/",
                            UriKind.Relative)));
        }
    }
}
