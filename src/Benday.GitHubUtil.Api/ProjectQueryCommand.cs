using Benday.CommandsFramework;
using Benday.GitHubUtil.Api.Messages.GetGraphQlMetadatas;
using Benday.GitHubUtil.Api.Messages.GetGraphQlTypeMetadatas;
using Benday.GitHubUtil.Api.Messages.ListProjectIterations;
using System.Text.Json;

namespace Benday.GitHubUtil.Api;

public abstract class ProjectQueryCommand : GitHubCommandBase
{
    protected ProjectQueryCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) :
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

    private string GetIterationsQuery()
    {
        return @"
query ($owner: String!, $number: Int!) {
  organization(login: $owner) {
    projectV2(number: $number) {
      fields(first: 100) {
        nodes {
          ... on ProjectV2IterationField {
            name
            configuration {
              iterations {
                id
                title
                startDate
                duration
              }
            }
          }
        }
      }
    }
  }
}
";
    }

    protected async Task<Iteration?> GetCurrentIteration(string projectName, string ownerId)
    {
        var response = await GetIterations(projectName, ownerId);
        if (response == null)
        {
            return null;
        }

        var iterations = response.Data.Organization.ProjectV2.Fields.Nodes;

        if (iterations == null)
        {
            return null;
        }


        foreach (var node in iterations)
        {
            if (string.IsNullOrEmpty(node.Name) == false)
            {
                foreach (var iteration in node.Configuration.Iterations)
                {
                    if (iteration.IsCurrentIteration() == true)
                    {
                        return iteration;
                    }
                }
            }
        }

        return null;
    }

    protected async Task<ListProjectIterationsResponse> GetIterations(string projectName, string ownerId)
    {
        // gh api graphql -F owner=<OWNER> -F number=<PROJECT_NUMBER> -f query='@iterations_query.graphql'
        var projectNumber = await GetProjectInfo(projectName, ownerId);

        if (projectNumber == null)
        {
            throw new KnownException("Could not find project.");
        }

        GitHubCliCommandRunner runner;


        var query = GetIterationsQuery();

        runner = new GitHubCliCommandRunner(_OutputProvider);

        runner.CommandName = "api";
        runner.SubCommandName = "graphql";
        runner.AddFieldArgument("owner", ownerId);
        runner.AddFieldArgument("number", projectNumber.Number.ToString());
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

            var response = JsonSerializer.Deserialize<ListProjectIterationsResponse>(runner.OutputText);

            if (response == null)
            {
                throw new InvalidOperationException("Could not deserialize output.");
            }

            return response;            
        }
    }
}
