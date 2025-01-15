using Benday.CommandsFramework;
using Benday.GitHubUtil.Api.Messages.GetGraphQlMetadatas;
using Benday.GitHubUtil.Api.Messages.ListProjectIterations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api;

[Command(
    Category = Constants.Category_Projects,
    Name = Constants.CommandName_GraphQlMetadata,
        Description = "Get graphql metadata",
        IsAsync = true)]
public class GraphQlMetadataCommand : GitHubCommandBase
{
  public GraphQlMetadataCommand(
      CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
  {
  }

  public override ArgumentCollection GetArguments()
  {
    var arguments = new ArgumentCollection();

    arguments.AddString("filter").AsNotRequired().
      WithDescription("Filter for the query.").
      WithDefaultValue(string.Empty);

    return arguments;
  }

  protected override async Task OnExecute()
  {
    WriteLine("Getting metadata...");

    var filter = Arguments.GetStringValue("filter");

    await GetMetadata(filter);
  }

  private async Task GetMetadata(string filter)
  {
    GitHubCliCommandRunner runner;

    var query = GetQuery();

    runner = new GitHubCliCommandRunner(_OutputProvider);

    runner.CommandName = "api";
    runner.SubCommandName = "graphql";
    runner.AddFieldArgument("query", query);

    await runner.RunAsync();

    if (runner.IsSuccess == false)
    {
      WriteLine("Error running gh command.");
      WriteLine(runner.ErrorText);
      throw new KnownException("Error running gh command.");
    }
    else if (string.IsNullOrWhiteSpace(runner.OutputText))
    {
      throw new KnownException("No output from gh command.");
    }
    else
    {
      var response = JsonSerializer.Deserialize<GetGraphQlMetadataResponse>(runner.OutputText);

      if (response == null)
      {
        throw new InvalidOperationException("Could not deserialize output.");
      }

      foreach (var node in response.Data.Schema.Types)
      {
        if (node.Name == null)
        {
          continue;
        }
        else if (string.IsNullOrWhiteSpace(filter) == false &&
            node.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase) == false)
        {
          continue;
        }

        WriteLine();
        WriteLine($"Type: {node.Name}");
        WriteLine($"Kind: {node.Kind}");
        WriteLine($"Description: {node.Description}");
      }
    }
  }

  private bool IsCurrentIteration(Iteration iteration)
  {
    var now = DateTime.Now;

    if (!DateTime.TryParse(iteration.StartDate, out var iterationStartDate))
    {
      return false;
    }

    if (iterationStartDate <= now && iterationStartDate.AddDays(iteration.Duration) >= now)
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  private string GetQuery()
  {
    return @"
query {
  __schema {
    types {
      name
      kind
      description
    }
    queryType {
      name
    }
    mutationType {
      name
    }
    subscriptionType {
      name
    }
    directives {
      name
      description
    }
  }
}
";
  }
}