using System;
using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api.Messages.GetGraphQlMetadatas;

public class GetGraphQlMetadataResponse
{
    [JsonPropertyName("data")]
    public Data Data { get; set; } = new();

}


public class Data
{
    [JsonPropertyName("__schema")]
    public Schema Schema { get; set; } = new();

}


public class Schema
{
    [JsonPropertyName("types")]
    public Type[] Types { get; set; } = new Type[0];

    [JsonPropertyName("queryType")]
    public QueryType QueryType { get; set; } = new();

    [JsonPropertyName("mutationType")]
    public MutationType MutationType { get; set; } = new();

    [JsonPropertyName("subscriptionType")]
    public string SubscriptionType { get; set; } = string.Empty;

    [JsonPropertyName("directives")]
    public Directive[] Directives { get; set; } = new Directive[0];

}


public class Type
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

}


public class QueryType
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

}


public class MutationType
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

}


public class Directive
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

}


