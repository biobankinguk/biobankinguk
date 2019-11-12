﻿using AutoMapper;
using Upload.Common.Data.Entities;
using Upload.DTOs;

namespace Upload.Profiles
{
    /// <inheritdoc />
    public class TreatmentProfile : Profile
    {
        /// <inheritdoc />
        public TreatmentProfile()
        {
            CreateMap<TreatmentSubmissionDto, TreatmentDto>();
            CreateMap<TreatmentOperationDto, TreatmentDto>();

            // base entities
            CreateMap<StagedTreatment, LiveTreatment>();
        }
    }
}
