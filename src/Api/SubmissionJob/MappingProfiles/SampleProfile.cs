﻿using AutoMapper;
using Biobanks.Common.Data.Entities;
using Biobanks.Common.Models;
using Biobanks.SubmissionJob.Dtos;

namespace Biobanks.SubmissionJob.MappingProfiles
{
    public class SampleProfile : Profile
    {
        public SampleProfile()
        {
            CreateMap<StagedSample, LiveSample>();
            CreateMap<SampleModel, SampleDto>();
            CreateMap<SampleIdModel, SampleIdDto>();
        }
    }
}
