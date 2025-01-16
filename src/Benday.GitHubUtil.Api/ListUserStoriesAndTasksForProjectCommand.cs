using Benday.CommandsFramework;
using Benday.GitHubUtil.Api.Messages.GetSubIssues;
using Benday.GitHubUtil.Api.Messages.ListProjectIssues;
using Benday.GitHubUtil.Api.Messages.ListProjectIterations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api;

[Command(
    Category = Constants.Category_Projects,
    Name = Constants.CommandName_ProjectItems,
        Description = "Get a list of user stories and tasks in the current iteration for a project",
        IsAsync = true)]
public class ListUserStoriesAndTasksForProjectCommand : ProjectQueryCommand
{
    public ListUserStoriesAndTasksForProjectCommand(
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
        var ownerId = Arguments.GetStringValue(Constants.CommandArg_OwnerId);
        var projectName = Arguments.GetStringValue(Constants.CommandArg_ProjectName);

        WriteLine("Getting iterations...");

        var currentIteration = await GetCurrentIteration(projectName, ownerId);

        var data = await GetItems(projectName, ownerId);

        var items = data.Items;

        /*
        if (currentIteration != null)
        {
            // filter but iteration

            items = items.Where(x => 
                x.Iteration.Title == currentIteration.Title && 
                x.Iteration.StartDate == currentIteration.StartDate).ToArray();
        }
        */

        var parents = await GetSubIssueData(items);

        var parentsWithChildrenNotInProject = parents.Where(x => x.OtherChildren.Count > 0).ToArray();

        if (parentsWithChildrenNotInProject.Length > 0)
        {
            WriteLine();
            WriteLine($"Found {parentsWithChildrenNotInProject.Length} parent issues with children not in project.");

            foreach (var parent in parentsWithChildrenNotInProject)
            {
                WriteLine($"Parent: {parent.Item.Title} -- {parent.Item.Id}");
                foreach (var child in parent.OtherChildren)
                {
                    WriteLine($"  Child: {child.Title} -- {child.Id}");
                }
            }
            WriteLine();
        }

        foreach (var item in parents)
        {
            WriteLine($"Parent: {item.Item.Title} -- {item.Item.Id}");

            if (item.Children.Count == 0)
            {
                WriteLine($"  No children.");
            }
            else
            {
                foreach (var child in item.Children)
                {
                    WriteLine($"  Child: {child.Title} -- {child.Id}");
                }
            }
        }
    }

    private async Task<List<GitHubParentIssue>> GetSubIssueData(Item[] items)
    {
        WriteLine($"Getting subissue data for {items.Length} items...");

        var parents = new List<GitHubParentIssue>();

        foreach (var item in items)
        {
            var result = await GetSubIssueData(item);

            if (result == null || result.Length == 0)
            {
                parents.Add(new GitHubParentIssue(item));
            }
            else
            {
                var parent = new GitHubParentIssue(item);

                parents.Add(parent);

                foreach (var subIssue in result)
                {
                    var target = items.Where(x => 
                        x.Content.Url == subIssue.HtmlUrl).FirstOrDefault();

                    if (target == null)
                    {
                        parent.AddChild(subIssue);
                    }
                    else
                    {
                        parent.AddChild(target);
                    }
                    
                }
            }
        }

        return parents;
    }

    private async Task<GetSubIssuesInfoResponse[]?> GetSubIssueData(Item item)
    {
        /*
         * # GitHub CLI api
# https://cli.github.com/manual/gh_api

gh api \
  -H "Accept: application/vnd.github+json" \
  -H "X-GitHub-Api-Version: 2022-11-28" \
  /repos/OWNER/REPO/issues/ISSUE_NUMBER/sub_issues
        */

        GitHubCliCommandRunner runner = new GitHubCliCommandRunner(_OutputProvider);

        runner.CommandName = "api";

        runner.AddHeaderArgument("Accept", "application/vnd.github+json");
        runner.AddHeaderArgument("X-GitHub-Api-Version", "2022-11-28");

        var url = $"/repos/{item.Content.Repository}/issues/{item.Content.Number}/sub_issues";

        runner.AddArgument(url);

        WriteLine();
        WriteLine($"Calling url: {url}");

        await runner.RunAsync();

        if (runner.IsSuccess == false)
        {
            WriteLine("Error running gh command.");
            WriteLine(runner.ErrorText);
            throw new KnownException("Error running gh command.");
        }
        else if (string.IsNullOrWhiteSpace(runner.OutputText))
        {
            // throw new KnownException("No output from gh command.");
            return null;
        }
        else
        {

            try
            {

                var response = JsonSerializer.Deserialize<GetSubIssuesInfoResponse[]>(runner.OutputText);

                if (response == null)
                {
                    throw new InvalidOperationException("Could not deserialize output.");
                }

                return response;
            }
            catch (Exception ex)
            {
                WriteLine("Error deserializing output.");
                WriteLine(ex.ToString());

                WriteLine();

                WriteLine(runner.OutputText);

                WriteLine();

                WriteLine();

                throw;
            }
        }
    }



    private async Task<ListProjectIssuesResponse> GetItems(string projectName, string ownerId)
    {
        var projectNumber = await GetProjectInfo(projectName, ownerId);

        if (projectNumber == null)
        {
            throw new KnownException("Could not find project.");
        }

        // gh project item-list 19 --owner benday-inc --format json

        GitHubCliCommandRunner runner = new GitHubCliCommandRunner(_OutputProvider);

        runner.CommandName = "project";

        runner.SubCommandName = "item-list";

        runner.FormatJson = true;

        runner.AddArgument(projectNumber.Number.ToString());
        runner.AddArgument("--owner", ownerId);

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
            var response = JsonSerializer.Deserialize<ListProjectIssuesResponse>(runner.OutputText);

            if (response == null)
            {
                throw new InvalidOperationException("Could not deserialize output.");
            }

            return response;
        }
    }

    private async Task GetItemsFromGraphQl(string projectName, string ownerId)
    {
        // gh api graphql -F owner=<OWNER> -F number=<PROJECT_NUMBER> -f query='@iterations_query.graphql'
        var projectNumber = await GetProjectInfo(projectName, ownerId);

        if (projectNumber == null)
        {
            throw new KnownException("Could not find project.");
        }

        GitHubCliCommandRunner runner;


        var query = GetQuery();

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

            foreach (var node in response.Data.Organization.ProjectV2.Fields.Nodes)
            {
                if (string.IsNullOrEmpty(node.Name) == false)
                {

                    WriteLine($"Field: {node.Name}");

                    foreach (var iteration in node.Configuration.Iterations)
                    {
                        WriteLine($"  Iteration: {iteration.Title}");
                        WriteLine($"    Id: {iteration.Id}");
                        WriteLine($"    Start Date: {iteration.StartDate}");
                        WriteLine($"    Duration: {iteration.Duration}");

                        var isCurrentIteration = IsCurrentIteration(iteration);

                        WriteLine($"    Is Current: {isCurrentIteration}");
                    }
                }
            }
        }
    }

    private bool IsCurrentIteration(Messages.ListProjectIterations.Iteration iteration)
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
query ($owner: String!, $projectName: String!) {
  organization(login: $owner) {
    projectNext(name: $projectName) {
      items(first: 100) {
        nodes {
          id
          title
          content {
            ... on Issue {
              id
              title
              number
              repository {
                name
                owner {
                  login
                }
              }
              labels(first: 10) {
                nodes {
                  name
                }
              }
              projectItems(first: 10) {
                nodes {
                  iteration {
                    id
                    title
                    state
                  }
                }
              }
              timelineItems(itemTypes: [CONNECTED_EVENT], first: 100) {
                nodes {
                  ... on ConnectedEvent {
                    subject {
                      ... on Issue {
                        id
                        title
                        number
                        labels(first: 10) {
                          nodes {
                            name
                          }
                        }
                        repository {
                          name
                          owner {
                            login
                          }
                        }
                      }
                    }
                  }
                }
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
