using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Search.Dto.Documents;
using Biobanks.Search.Dto.PartialDocuments;

namespace Biobanks.Search.Contracts
{
    public interface ICapabilityIndexProvider
    {
        void Index(int id, CapabilityDocument capabilitySearch);
        void Update(int id, PartialCapability partialCapability);
        void Update(int id, PartialBiobank partialBiobank);
        void Update(int id, PartialNetworks partialNetworks);
        void Delete(int id);

        void Index(IEnumerable<CapabilityDocument> capabilitySearchDocuments);
        void Delete(IEnumerable<int> capabilitySearchDocumentIds);

        /// <summary>
        /// Completely empty the CapabilitySearchDocument index
        /// </summary>
        Task ClearAsync();
    }
}