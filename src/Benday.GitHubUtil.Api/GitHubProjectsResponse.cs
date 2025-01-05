using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api;



public class GitHubProjectsResponse
{
    [JsonPropertyName("projects")]
    public GitHubProject[] Projects { get; set; } = new GitHubProject[0];

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

}
