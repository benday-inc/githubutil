using Benday.CommandsFramework;
using Benday.GitHubUtil.Api.Messages.ListProjectIterations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api;

[Command(
    Category = Constants.Category_Projects,
    Name = Constants.CommandName_ProjectIterations,
        Description = "Get a list of iterations for a project",
        IsAsync = true)]
public class ListProjectIterationsCommand : GitHubCommandBase
{
    public ListProjectIterationsCommand(
        CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        var arguments = new ArgumentCollection();

        arguments.AddString(Constants.CommandArg_OwnerId).AsRequired().WithDescription("Owner id.");

        arguments.AddString(Constants.CommandArg_ProjectName).WithDescription("Name of the project to use.");

        return arguments;
    }

    protected override async Task OnExecute()
    {
        WriteLine("Getting iterations...");

        var ownerId = Arguments.GetStringValue(Constants.CommandArg_OwnerId);
        var projectName = Arguments.GetStringValue(Constants.CommandArg_ProjectName);

        await GetIterations(projectName, ownerId);
    }

    private async Task GetIterations(string projectName, string ownerId)
    {
        // gh api graphql -F owner=<OWNER> -F number=<PROJECT_NUMBER> -f query='@iterations_query.graphql'
        var projectNumber = await GetProjectInfo(projectName, ownerId);

        if (projectNumber == null)
        {
            throw new KnownException("Could not find project.");
        }

        GitHubCliCommandRunner runner;

        if (1 == 0)
        {
            var query = GetSimpleQuery();

            runner = new GitHubCliCommandRunner(_OutputProvider);

            runner.CommandName = "api";
            runner.SubCommandName = "graphql";
            runner.AddFieldArgument("query", query);

            await runner.RunAsync();
        }
        else
        {
            var query = GetQuery();

            runner = new GitHubCliCommandRunner(_OutputProvider);

            runner.CommandName = "api";
            runner.SubCommandName = "graphql";
            runner.AddFieldArgument("owner", ownerId);
            runner.AddFieldArgument("number", projectNumber.Number.ToString());
            runner.AddFieldArgument("query", query);

            await runner.RunAsync();
        }

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

            foreach (var node in response.Data.Organization.ProjectV2.Fields.Nodes)
            {
                WriteLine($"Field: {node.Name}");

                foreach (var iteration in node.Configuration.Iterations)
                {
                    WriteLine($"  Iteration: {iteration.Title}");
                    WriteLine($"    Id: {iteration.Id}");
                    WriteLine($"    Start Date: {iteration.StartDate}");
                    WriteLine($"    Duration: {iteration.Duration}");
                }
            }
        }
    }

    private string GetSimpleQuery()
    {
        return "query { viewer { login } }";
    }

    private string GetQuery()
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
}