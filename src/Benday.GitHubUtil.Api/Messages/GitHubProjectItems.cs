using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api.Messages;


public class GitHubProjectItems
{
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

}
