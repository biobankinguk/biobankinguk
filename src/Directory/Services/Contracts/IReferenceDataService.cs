using Biobanks.Entities.Data.ReferenceData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Contracts
{
    public interface IReferenceDataService
    {
        Task<ICollection<T>> List<T>() where T : ReferenceDataBase;

        Task<T> Get<T>(int id) where T : ReferenceDataBase;

        Task<bool> Exists<T>(string value) where T : ReferenceDataBase;

    }
}
