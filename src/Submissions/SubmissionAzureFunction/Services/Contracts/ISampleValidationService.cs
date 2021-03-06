﻿using System.Threading.Tasks;
using Biobanks.SubmissionAzureFunction.Dtos;

using StagedSample = Biobanks.LegacyData.Entities.StagedSample;

namespace Biobanks.SubmissionAzureFunction.Services.Contracts
{
    public interface ISampleValidationService
    {
        Task<StagedSample> ValidateAndPopulateSample(SampleDto dto, StagedSample sample = null);
    }
}
