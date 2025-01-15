using System;
using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api.Messages.GetGraphQlTypeMetadatas;

public class GetGraphQlTypeMetadataResponse
{
    [JsonPropertyName("data")]
    public Data Data { get; set; } = new();

}


public class Data
{
    [JsonPropertyName("__type")]
    public Type Type { get; set; } = new();

}


public class Type
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("fields")]
    public Field[] Fields { get; set; } = new Field[0];

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = string.Empty;

}


public class Field
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public Type Type { get; set; } = new();

    [JsonPropertyName("args")]
    public Arg[] Args { get; set; } = new Arg[0];

}


public class Arg
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public Type Type { get; set; } = new();

}



