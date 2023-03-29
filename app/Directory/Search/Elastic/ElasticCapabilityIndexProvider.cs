using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Directory.Search.Contracts;
using Biobanks.Directory.Search.Dto.Documents;
using Biobanks.Directory.Search.Dto.PartialDocuments;
using Elasticsearch.Net;
using Nest;

namespace Biobanks.Directory.Search.Elastic
{
    public class ElasticCapabilityIndexProvider : BaseElasticIndexProvider, ICapabilityIndexProvider
    {
        private readonly ElasticClient _client;

        public ElasticCapabilityIndexProvider(
            string elasticSearchUrl,
            (string collections, string capabilities) indexNames,
            string username,
            string password)
        {
            var node = new Uri(elasticSearchUrl);
            var pool = new SingleNodeConnectionPool(node);

            var settings = new ConnectionSettings(pool)
                .DefaultMappingFor<CollectionDocument>(
                    m => m.IndexName(indexNames.collections))
                .DefaultMappingFor<CapabilityDocument>(
                    m => m.IndexName(indexNames.capabilities));

            // If there's a username and password for Basic Auth, use it
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                settings.BasicAuthentication(username, password);
            }
            
            settings.EnableApiVersioningHeader();

            _client = new ElasticClient(settings);
        }

        [DisplayName("Index CapabilityDocument (Id = {0})")]
        public void Index(int id, CapabilityDocument capabilitySearch)
        {
            var indexResponse = _client.Index(capabilitySearch, i => i.Refresh(null));

            HandleIndexResponse(indexResponse);
        }

        [DisplayName("Update CapabilityDocument (Id = {0})")]
        public void Update(int id, PartialCapability partialCapability)
        {
            var indexResponse = _client.Update<CapabilityDocument, PartialCapability>(
                DocumentPath<CapabilityDocument>.Id(id),
                x => x
                    .Doc(partialCapability)
                    .RetryOnConflict(3)
                    .Refresh(null));

            HandleIndexResponse(indexResponse);
        }

        [DisplayName("Update CapabilityDocument (Id = {0}) because the Biobank was updated")]
        public void Update(int id, PartialBiobank partialBiobank)
        {
            var indexResponse = _client.Update<CapabilityDocument, PartialBiobank>(
                DocumentPath<CapabilityDocument>.Id(id),
                x => x
                    .Doc(partialBiobank)
                    .RetryOnConflict(3)
                    .Refresh(null));

            HandleIndexResponse(indexResponse);
        }

        [DisplayName("Update CapabilityDocument (Id = {0}) because the Networks were updated")]
        public void Update(int id, PartialNetworks partialNetworks)
        {
            var indexResponse = _client.Update<CapabilityDocument, PartialNetworks>(
                DocumentPath<CapabilityDocument>.Id(id),
                x => x
                    .Doc(partialNetworks)
                    .RetryOnConflict(3)
                    .Refresh(null));

            HandleIndexResponse(indexResponse);
        }

        [DisplayName("Delete CapabilityDocument (Id = {0})")]
        public void Delete(int id)
        {
            var indexResponse = _client.Delete<CapabilityDocument>(id);

            HandleIndexResponse(indexResponse);
        }

        [DisplayName("Bulk index CapabilityDocuments")]
        public void Index(IEnumerable<CapabilityDocument> capabilityDocuments)
        {
            capabilityDocuments = capabilityDocuments.ToList(); //enumerate once only

            if (!capabilityDocuments.Any()) return; //return if empty

            var indexResponse = _client.IndexMany(capabilityDocuments);

            HandleIndexResponse(indexResponse);
        }

        [DisplayName("Bulk delete CapabilityDocuments")]
        public void Delete(IEnumerable<int> capabilityDocumentIds)
        {
            var capabilityDocuments = capabilityDocumentIds.Select(x => new CapabilityDocument { Id = x }).ToList(); //enumerate once only

            if (!capabilityDocuments.Any()) return; //return if empty

            var indexResponse = _client.DeleteMany(capabilityDocuments);

            HandleIndexResponse(indexResponse);
        }

        /// <inheritdoc />
        [DisplayName("Clear the CapabilityDocument Index")]
        public async Task ClearAsync()
            => await _client.DeleteByQueryAsync<CapabilityDocument>(
                x => x.Query(q => q.QueryString(qs => qs.Query("*"))));
    }
}
