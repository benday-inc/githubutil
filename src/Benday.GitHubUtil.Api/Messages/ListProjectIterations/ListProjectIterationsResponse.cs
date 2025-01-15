using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api.Messages.ListProjectIterations;

public class ListProjectIterationsResponse
{
    [JsonPropertyName("data")]
    public Data Data { get; set; } = new();

}


public class Data
{
    [JsonPropertyName("organization")]
    public Organization Organization { get; set; } = new();

}


public class Organization
{
    [JsonPropertyName("projectV2")]
    public ProjectV2 ProjectV2 { get; set; } = new();

}


public class ProjectV2
{
    [JsonPropertyName("fields")]
    public Fields Fields { get; set; } = new();

}


public class Fields
{
    [JsonPropertyName("nodes")]
    public Node[] Nodes { get; set; } = new Node[0];

}


public class Node
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("configuration")]
    public Configuration Configuration { get; set; } = new();

}


public class Configuration
{
    [JsonPropertyName("iterations")]
    public Iteration[] Iterations { get; set; } = new Iteration[0];

}


public class Iteration
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; } = string.Empty;

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

}


