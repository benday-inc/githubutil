using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api.Messages;


public class GitHubFieldInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public GithubFieldOption[] Options { get; set; } = new GithubFieldOption[0];

}
