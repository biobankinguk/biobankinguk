namespace Biobanks.Directory.Config;

/// <summary>
/// Configurations values for the Elasticsearch 
/// </summary>
public class ElasticSearchConfig
{
    public string ApiBaseUrl { get; init; } = "http://localhost:9200";
    public string DefaultCollectionsSearchIndex { get; init; } = "collections";
    public string DefaultCapabilitiesSearchIndex { get; init; } = "capabilities";
    public string ApiKeyId { get; init; } = "";
    public string ApiKey { get; init; } = "";
}
