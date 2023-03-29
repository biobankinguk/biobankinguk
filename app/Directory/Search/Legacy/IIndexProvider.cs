using System.Collections.Generic;
using Biobanks.Directory.Search.Dto.Documents;
using Biobanks.Directory.Search.Dto.PartialDocuments;

namespace Biobanks.Directory.Search.Legacy
{
    // TODO: This is a legacy contract for bridging to the old index service code
    // It should be thrown away when the services are rewritten
    public interface IIndexProvider
    {
        void IndexCollectionSearchDocument(int id, CollectionDocument collectionSearch);
        void UpdateCollectionSearchDocument(int id, PartialSampleSet partialSampleSet);
        void UpdateCollectionSearchDocument(int id, PartialCollection partialCollection);
        void UpdateCollectionSearchDocument(int id, PartialBiobank partialBiobank);
        void UpdateCollectionSearchDocument(int id, PartialNetworks partialNetworks);
        void DeleteCollectionSearchDocument(int id);

        void IndexCapabilitySearchDocument(int id, CapabilityDocument capabilitySearch);
        void UpdateCapabilitySearchDocument(int id, PartialCapability partialCapability);
        void UpdateCapabilitySearchDocument(int id, PartialBiobank partialBiobank);
        void UpdateCapabilitySearchDocument(int id, PartialNetworks partialNetworks);
        void DeleteCapabilitySearchDocument(int id);

        void BulkIndexCollectionSearchDocuments(IEnumerable<CollectionDocument> collectionSearchDocuments);
        void BulkIndexCapabilitySearchDocuments(IEnumerable<CapabilityDocument> capabilitySearchDocuments);
        void BulkDeleteCollectionSearchDocuments(IEnumerable<int> collectionSearchDocumentIds);
        void BulkDeleteCapabilitySearchDocuments(IEnumerable<int> capabilitySearchDocumentIds);
    }
}
