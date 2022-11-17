using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts;

public interface ISampleSetService
{

  Task<int> GetSampleSetCountAsync();
  Task<int> GetIndexableSampleSetCountAsync();
  Task<int> GetSuspendedSampleSetCountAsync();
  Task<IEnumerable<int>> GetAllSampleSetIdsAsync();

}
