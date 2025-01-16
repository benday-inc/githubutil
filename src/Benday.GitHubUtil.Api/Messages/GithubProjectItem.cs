using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api.Messages;


public class GithubProjectItem
{
    [JsonPropertyName("content")]
    public GithubProjectContent Content { get; set; } = new();

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("estimate")]
    public int Estimate { get; set; }

}


