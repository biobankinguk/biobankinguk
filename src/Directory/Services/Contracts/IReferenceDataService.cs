using Biobanks.Entities.Data.ReferenceData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Contracts
{
    public interface IReferenceDataService<T> where T : ReferenceDataBase
    {
        Task Add(T entity);

        Task<int> Count();

        Task Delete(int id);

        Task<bool> Exists(string value);

        Task<T> Get(int id);

        Task<ICollection<T>> List();

        Task<T> Update(T entity);
    }
}
