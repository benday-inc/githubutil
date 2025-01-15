using Benday.CommandsFramework;
using Benday.GitHubUtil.Api.Messages.GetGraphQlTypeMetadatas;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api;

[Command(
    Category = Constants.Category_Projects,
    Name = Constants.CommandName_GraphQlTypeMetadata,
        Description = "Get graphql metadata for a type",
        IsAsync = true)]
public class GraphQlTypeMetadataCommand : GitHubCommandBase
{
  public GraphQlTypeMetadataCommand(
      CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
  {
  }

  public override ArgumentCollection GetArguments()
  {
    var arguments = new ArgumentCollection();

    arguments.AddString(Constants.CommandArg_TypeName).
      AsRequired().
      WithDescription("Name of the type to get metadata for.");

    return arguments;
  }

  protected override async Task OnExecute()
  {
    WriteLine("Getting metadata for type...");

    var typeName = Arguments.GetStringValue(Constants.CommandArg_TypeName);

    await GetMetadata(typeName);
  }

  private async Task GetMetadata(string typeName)
  {
    GitHubCliCommandRunner runner;

    var query = GetQuery();

    runner = new GitHubCliCommandRunner(_OutputProvider);

    runner.CommandName = "api";
    runner.SubCommandName = "graphql";
    runner.AddFieldArgument("typeName", typeName);
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
      var response = JsonSerializer.Deserialize<GetGraphQlTypeMetadataResponse>(runner.OutputText);

      if (response == null)
      {
        throw new InvalidOperationException("Could not deserialize output.");
      }

      foreach (var node in response.Data.Type.Fields)
      {
        WriteLine();
        WriteLine($"Type: {node.Name}");
        WriteLine($"Description: {node.Description}");
        WriteLine($"Type.Kind: {node.Type.Kind}");
        WriteLine($"Type.Name: {node.Type.Name}");

        if (node.Args != null && node.Args.Length > 0)
        {
          WriteLine("Args:");
          foreach (var arg in node.Args)
          {
            WriteLine($"  Arg: {arg.Name}");
            WriteLine($"  Arg.Description: {arg.Description}");
            WriteLine($"  Arg.Type.Kind: {arg.Type.Kind}");
            WriteLine($"  Arg.Type.Name: {arg.Type.Name}");
          }
        }
        else
        {
          WriteLine("Args:");
          WriteLine("  No args.");
        }
      }
    }
  }

  private string GetQuery()
  {
    return @"
query GetTypeMetadata($typeName: String!) {
  __type(name: $typeName) {
    name
    description
    fields {
      name
      description
      args {
        name
        description
        type {
          name
          kind
        }
      }
      type {
        name
        kind
      }
    }
  }
}
";
  }
}