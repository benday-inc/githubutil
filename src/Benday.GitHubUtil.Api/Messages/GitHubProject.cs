using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api;


public class GitHubProject
{
    [JsonPropertyName("closed")]
    public bool Closed { get; set; }

    [JsonPropertyName("fields")]
    public GithubProjectFields Fields { get; set; } = new();

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("items")]
    public GitHubProjectItems Items { get; set; } = new();

    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("owner")]
    public GithubProjectOwner Owner { get; set; } = new();

    [JsonPropertyName("public")]
    public bool Public { get; set; }

    [JsonPropertyName("readme")]
    public string Readme { get; set; } = string.Empty;

    [JsonPropertyName("shortDescription")]
    public string ShortDescription { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

}
