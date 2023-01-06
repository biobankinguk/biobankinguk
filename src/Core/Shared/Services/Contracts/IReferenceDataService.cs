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

        /// <summary>
        /// The number of times the entity is used or referenced
        /// </summary>
        /// <param name="id">The Id of the entity <typeparamref name="T"/></param>
  }
}
