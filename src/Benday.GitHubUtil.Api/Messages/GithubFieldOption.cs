using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api.Messages;


public class GithubFieldOption
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

}
