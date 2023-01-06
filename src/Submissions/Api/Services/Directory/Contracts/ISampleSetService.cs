using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts;

public interface ISampleSetService
{
  Task AddSampleSetAsync(SampleSet sampleSet);
  Task UpdateSampleSetAsync(SampleSet sampleSet);
  Task DeleteSampleSetAsync(int id);

  Task<int> GetSampleSetCountAsync();
  Task<IEnumerable<int>> GetAllSampleSetIdsAsync();
  Task<int> GetIndexableSampleSetCountAsync();
  Task<int> GetSuspendedSampleSetCountAsync();
  Task<SampleSet> GetSampleSetByIdAsync(int id);
}
