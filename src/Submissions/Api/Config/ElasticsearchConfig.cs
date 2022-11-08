namespace Biobanks.Submissions.Api.Config;

/// <summary>
/// Configurations values for the Elasticsearch 
/// </summary>
public class ElasticsearchConfig
{
    public string ElasticsearchUrl { get; init; } = "http://localhost:9200";
    public string DefaultCollectionsSearchIndex { get; init; } = "collections";
    public string DefaultCapabilitiesSearchIndex { get; init; } = "capabilities";
    public string ElasticsearchUsername { get; init; } = "";
    public string ElasticsearchPassword { get; init; } = "";
}
