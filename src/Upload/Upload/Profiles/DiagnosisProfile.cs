﻿using AutoMapper;
using Upload.Common.Data.Entities;
using Upload.DTO;

namespace Upload.Profiles
{
    /// <inheritdoc />
    public class DiagnosisProfile : Profile
    {
        /// <inheritdoc />
        public DiagnosisProfile()
        {
            CreateMap<DiagnosisIdDto, DiagnosisDto>();
            CreateMap<DiagnosisOperationDto, DiagnosisDto>();

            // base entities
            CreateMap<StagedDiagnosis, LiveDiagnosis>();
        }
    }
}
