using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Directory.Search.Dto.Documents;
using Biobanks.Directory.Search.Dto.PartialDocuments;

namespace Biobanks.Directory.Search.Contracts
{
    public interface ICollectionIndexProvider
    {
        void Index(int id, CollectionDocument collectionSearch);
        void Update(int id, PartialSampleSet partialSampleSet);
        void Update(int id, PartialCollection partialCollection);
        void Update(int id, PartialBiobank partialBiobank);
        void Update(int id, PartialNetworks partialNetworks);
        void Delete(int id);

        void Index(IEnumerable<CollectionDocument> collectionSearchDocuments);
        void Delete(IEnumerable<int> collectionSearchDocumentIds);

        /// <summary>
        /// Completely empty the CollectionSearchDocument index
        /// </summary>
        Task ClearAsync();
    }
}
