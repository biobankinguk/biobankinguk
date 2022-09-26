using Biobanks.Entities.Data.ReferenceData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Shared.Services.Contracts
{
    public interface IReferenceDataService<T> where T : BaseReferenceData
    {
        /// <summary>
        /// Get all untracked entities
        /// </summary>
        /// <returns>An ICollection of all the entities</returns>
        Task<ICollection<T>> List();

        /// <summary>
        /// Get all untracked entities
        /// </summary>
        /// <returns>An ICollection of all the entities</returns>
        Task<ICollection<T>> List(string wildcard);
    }
}
