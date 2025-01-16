using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api.Messages;


public class GithubProjectFields
{
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

}
