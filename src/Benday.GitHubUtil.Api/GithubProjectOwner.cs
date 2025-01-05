using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api;


public class GithubProjectOwner
{
    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

}
