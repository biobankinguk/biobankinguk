using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Api.ReferenceData;

namespace Biobanks.SubmissionApi.Services.Contracts
{
    /// <summary>
    /// Service for handling Sample Content Method reference data.
    /// </summary>
    public interface ISampleContentMethodService
    {
        /// <summary>
        /// Get a Sample Content Method entity.
        /// </summary>
        /// <param name="sampleContentMethodId">The ID of the Sample Content Method to fetch.</param>
        /// <returns>The requested Sample Content Method.</returns>
        Task<SampleContentMethod> Get(int sampleContentMethodId);

        /// <summary>
        /// Get a list of Sample Content Method entities.
        /// </summary>
        /// <returns>A list of Sample Content Method entities.</returns>
        Task<IEnumerable<SampleContentMethod>> List();

        /// <summary>
        /// Inserts a new Sample Content Method entity into the repository.
        /// </summary>
        /// <param name="sampleContentMethod">The Sample Content Method object to insert.</param>
        /// <returns>The created Sample Content Method.</returns>
        Task<SampleContentMethod> Create(SampleContentMethod sampleContentMethod);
        
        /// <summary>
        /// Updates an existing Sample Content Method entity in the respository.
        /// </summary>
        /// <param name="sampleContentMethod">The Sample Content Method object to update.</param>
        /// <returns></returns>
        Task Update(SampleContentMethod sampleContentMethod);

        /// <summary>
        /// Get a Sample Content Method entity.
        /// </summary>
        /// <param name="value">The value of the Sample Content Method to fetch.</param>
        /// <returns>The requested Sample Content Method.</returns>
        Task<SampleContentMethod> GetByValue(string value);
    }
}