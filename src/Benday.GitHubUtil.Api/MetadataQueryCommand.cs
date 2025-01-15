using Benday.CommandsFramework;
using Benday.GitHubUtil.Api.Messages.GetGraphQlMetadatas;
using Benday.GitHubUtil.Api.Messages.GetGraphQlTypeMetadatas;
using System.Text.Json;

namespace Benday.GitHubUtil.Api;

public abstract class MetadataQueryCommand : GitHubCommandBase
{
    protected MetadataQueryCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) :
        base(info, outputProvider)
    {
    }

    protected async Task<GetGraphQlTypeMetadataResponse> GetTypeMetadata(string typeName, string filter, bool quiet = false)
    {
        GitHubCliCommandRunner runner;

        var query = GetTypeMetadataQuery();

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

            if (quiet == true)
            {
                return response;
            }

            foreach (var node in response.Data.Type.Fields)
            {
                if (string.IsNullOrWhiteSpace(filter) == false)
                {
                    var nodeNameStartsWithFilter = node.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase);

                    var nodeArgNameStartsWithFilter = false;

                    if (node.Args != null && node.Args.Length > 0)
                    {
                        foreach (var arg in node.Args)
                        {
                            if (arg.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
                            {
                                nodeArgNameStartsWithFilter = true;
                                break;
                            }

                            if (arg.Type != null && arg.Type.Name != null &&
                              arg.Type.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
                            {
                                nodeArgNameStartsWithFilter = true;
                                break;
                            }
                        }
                    }

                    if (nodeNameStartsWithFilter == false && nodeArgNameStartsWithFilter == false)
                    {
                        // WriteLine($"Skipping {node.Name} because it does not match filter.");
                        continue;
                    }
                }

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


            return response;
        }
    }

    protected async Task<GetGraphQlMetadataResponse> GetMetadata(string filter, bool quiet = false)
    {
        GitHubCliCommandRunner runner;

        var query = GetMetadataQuery();

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

            if (quiet == false)
            {
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

                return response;
            }
            else
            {
                return response;
            }
        }


    }



    private string GetMetadataQuery()
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

    private string GetTypeMetadataQuery()
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
