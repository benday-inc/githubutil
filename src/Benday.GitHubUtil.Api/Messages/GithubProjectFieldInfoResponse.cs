using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api.Messages;


public class GithubProjectFieldInfoResponse
{
    [JsonPropertyName("fields")]
    public GitHubFieldInfo[] Fields { get; set; } = new GitHubFieldInfo[0];

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

}
