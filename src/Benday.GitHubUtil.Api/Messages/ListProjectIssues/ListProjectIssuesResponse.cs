using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api.Messages.ListProjectIssues;

public class ListProjectIssuesResponse
{
    [JsonPropertyName("items")]
    public Item[] Items { get; set; } = new Item[0];

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

}


public class Item
{
    [JsonPropertyName("content")]
    public Content Content { get; set; } = new();

    [JsonPropertyName("estimate")]
    public int Estimate { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("iteration")]
    public Iteration Iteration { get; set; } = new();

    [JsonPropertyName("repository")]
    public string Repository { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

}


public class Content
{
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("repository")]
    public string Repository { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

}


public class Iteration
{
    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

}


