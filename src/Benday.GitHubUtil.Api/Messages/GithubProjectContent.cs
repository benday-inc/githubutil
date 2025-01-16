using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api.Messages;


public class GithubProjectContent
{
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

}


