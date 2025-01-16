using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Benday.GitHubUtil.Api.Messages;
public class GithubProjectItemsResponse
{
    [JsonPropertyName("items")]
    public GithubProjectItem[] Items { get; set; } = new GithubProjectItem[0];

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

}


