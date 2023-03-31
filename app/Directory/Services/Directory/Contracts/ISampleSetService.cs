using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Data.Entities;

namespace Biobanks.Directory.Services.Directory.Contracts;

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
