using System.Collections.Generic;
using Biobanks.Search.Contracts;
using Biobanks.Search.Dto.Documents;
using Biobanks.Search.Dto.PartialDocuments;

namespace Biobanks.Search.Legacy
{
    // Implements the legacy interface used by the current Directory services
    // Just a wrapper for the new interfaces
    // TODO: Delete when the services are amended
    public class LegacyIndexProvider : IIndexProvider
    {
        private readonly ICapabilityIndexProvider _capabilities;
        private readonly ICollectionIndexProvider _collections;

        public LegacyIndexProvider(ICapabilityIndexProvider capabilities, ICollectionIndexProvider collections)
        {
            _capabilities = capabilities;
            _collections = collections;
        }

        public void BulkDeleteCapabilitySearchDocuments(IEnumerable<int> capabilitySearchDocumentIds)
            => _capabilities.Delete(capabilitySearchDocumentIds);

        public void BulkDeleteCollectionSearchDocuments(IEnumerable<int> collectionSearchDocumentIds)
            => _collections.Delete(collectionSearchDocumentIds);

        public void BulkIndexCapabilitySearchDocuments(IEnumerable<CapabilityDocument> capabilitySearchDocuments)
            => _capabilities.Index(capabilitySearchDocuments);

        public void BulkIndexCollectionSearchDocuments(IEnumerable<CollectionDocument> collectionSearchDocuments)
            => _collections.Index(collectionSearchDocuments);

        public void DeleteCapabilitySearchDocument(int id)
            => _capabilities.Delete(id);

        public void DeleteCollectionSearchDocument(int id)
            => _collections.Delete(id);

        public void IndexCapabilitySearchDocument(int id, CapabilityDocument capabilitySearch)
            => _capabilities.Index(id, capabilitySearch);

        public void IndexCollectionSearchDocument(int id, CollectionDocument collectionSearch)
            => _collections.Index(id, collectionSearch);

        public void UpdateCapabilitySearchDocument(int id, PartialCapability partialCapability)
            => _capabilities.Update(id, partialCapability);

        public void UpdateCapabilitySearchDocument(int id, PartialBiobank partialBiobank)
            => _capabilities.Update(id, partialBiobank);

        public void UpdateCapabilitySearchDocument(int id, PartialNetworks partialNetworks)
            => _capabilities.Update(id, partialNetworks);

        public void UpdateCollectionSearchDocument(int id, PartialSampleSet partialSampleSet)
            => _collections.Update(id, partialSampleSet);

        public void UpdateCollectionSearchDocument(int id, PartialCollection partialCollection)
            => _collections.Update(id, partialCollection);

        public void UpdateCollectionSearchDocument(int id, PartialBiobank partialBiobank)
            => _collections.Update(id, partialBiobank);

        public void UpdateCollectionSearchDocument(int id, PartialNetworks partialNetworks)
            => _collections.Update(id, partialNetworks);
    }
}
