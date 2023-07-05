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
  public class ElasticCollectionIndexProvider : BaseElasticIndexProvider, ICollectionIndexProvider
  {
    private readonly ElasticClient _client;

    public ElasticCollectionIndexProvider(
        string elasticSearchUrl,
        (string collections, string capabilities) indexNames,
        string apiKeyId,
        string apiKey)
    {
      var node = new Uri(elasticSearchUrl);
      var pool = new SingleNodeConnectionPool(node);

      var settings = new ConnectionSettings(pool)
          .DefaultMappingFor<CollectionDocument>(
              m => m.IndexName(indexNames.collections))
          .DefaultMappingFor<CapabilityDocument>(
              m => m.IndexName(indexNames.capabilities));

      // If there's an Api Key for Auth, use it
      if (!string.IsNullOrWhiteSpace(apiKeyId) && !string.IsNullOrWhiteSpace(apiKey))
      {
        settings.ApiKeyAuthentication(apiKeyId, apiKey);
      }

      settings.EnableApiVersioningHeader();

      _client = new ElasticClient(settings);
    }

    [DisplayName("Index CollectionDocument (Id = {0})")]
    public void Index(int id, CollectionDocument collectionSearch)
    {
      var indexResponse = _client.Index(collectionSearch, i => i.Refresh(null));

      HandleIndexResponse(indexResponse);
    }

    [DisplayName("Update CollectionDocument (Id = {0})")]
    public void Update(int id, PartialSampleSet partialSampleSet)
    {
      var indexResponse = _client.Update<CollectionDocument, PartialSampleSet>(
          DocumentPath<CollectionDocument>.Id(id),
          x => x.Doc(partialSampleSet)
              .RetryOnConflict(3)
              .Refresh(null));

      HandleIndexResponse(indexResponse);
    }

    [DisplayName("Update CollectionDocument (Id = {0}) because the Collection was updated")]
    public void Update(int id, PartialCollection partialCollection)
    {
      var indexResponse = _client.Update<CollectionDocument, PartialCollection>(
          DocumentPath<CollectionDocument>.Id(id),
          x => x
              .Doc(partialCollection)
              .RetryOnConflict(3)
              .Refresh(null));

      HandleIndexResponse(indexResponse);
    }

    [DisplayName("Update CollectionDocument (Id = {0}) because the Biobank was updated")]
    public void Update(int id, PartialBiobank partialBiobank)
    {
      var indexResponse = _client.Update<CollectionDocument, PartialBiobank>(
          DocumentPath<CollectionDocument>.Id(id),
          x => x
              .Doc(partialBiobank)
              .RetryOnConflict(3)
              .Refresh(null));

      HandleIndexResponse(indexResponse);
    }

    [DisplayName("Update CollectionDocument (Id = {0}) because the Networks were updated")]
    public void Update(int id, PartialNetworks partialNetworks)
    {
      var indexResponse = _client.Update<CollectionDocument, PartialNetworks>(
          DocumentPath<CollectionDocument>.Id(id),
          x => x
              .Doc(partialNetworks)
              .RetryOnConflict(3)
              .Refresh(null));

      HandleIndexResponse(indexResponse);
    }

    [DisplayName("Delete CollectionDocument (Id = {0})")]
    public void Delete(int id)
    {
      var indexResponse = _client.Delete<CollectionDocument>(id);

      HandleIndexResponse(indexResponse);
    }

    [DisplayName("Bulk index CollectionDocuments")]
    public void Index(IEnumerable<CollectionDocument> collectionDocuments)
    {
      collectionDocuments = collectionDocuments.ToList(); //enumerate once only

      if (!collectionDocuments.Any()) return; //return if empty

      var indexResponse = _client.IndexMany(collectionDocuments);

      HandleIndexResponse(indexResponse);
    }

    [DisplayName("Bulk delete CollectionDocuments")]
    public void Delete(IEnumerable<int> collectionDocumentIds)
    {
      var collectionDocuments = collectionDocumentIds.Select(x => new CollectionDocument { Id = x }).ToList(); //enumerate once only

      if (!collectionDocuments.Any()) return; //return if empty

      var indexResponse = _client.DeleteMany(collectionDocuments);

      HandleIndexResponse(indexResponse);
    }

    /// <inheritdoc />
    [DisplayName("Clear the CollectionDocument Index")]
    public async Task ClearAsync()
        => await _client.DeleteByQueryAsync<CollectionDocument>(
            x => x.Query(q => q.QueryString(qs => qs.Query("*"))));
  }
}
