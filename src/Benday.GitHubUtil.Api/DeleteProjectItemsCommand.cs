using Benday.CommandsFramework;


using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api;

[Command(
    Category = Constants.Category_TestData,
    Name = Constants.CommandName_ProjectIssuesDelete,
        Description = "Delete issues from project",
        IsAsync = true)]
public class DeleteProjectItemsCommand : GitHubCommandBase
{
    public DeleteProjectItemsCommand(
        CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        var arguments = new ArgumentCollection();

        arguments.AddString("projectname").AllowEmptyValue().AsNotRequired().WithDescription("Name of the project to use.");


        arguments.AddString("ownerid").AllowEmptyValue().AsNotRequired().WithDescription("Owner id.");

        return arguments;
    }

    protected override async Task OnExecute()
    {
        var projectName = Arguments.GetStringValue("projectname");
        var ownerId = Arguments.GetStringValue("ownerid");

        var project = await GetProjectInfo(projectName, ownerId);

        if (project == null)
        {
            throw new KnownException("Could not find project.");
        }

        var items = await GetItemsAsync(project);

        if (items.TotalCount > 0)
        {
            foreach (var item in items.Items)
            {
                WriteLine($"Deleting item {item.Title}...");
                await DeleteItemAsync(project, item);
            }
        }
        else
        {
            WriteLine("No items to delete.");
        }
    }
private Task DeleteItemAsync(GitHubProject project, GithubProjectItem item)
    {
        var template = $"project item-delete {project.Number} --id {item.Id} --owner {project.Owner.Login} --format json";

        // call gh to get the items
        var processStartInfo = new ProcessStartInfo("gh", template)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        string output = string.Empty;
        string error = string.Empty;
        using var process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        output = process.StandardOutput.ReadToEnd();
        error = process.StandardError.ReadToEnd();
        if (!string.IsNullOrEmpty(error))
        {
            WriteLine($"Error: {error}");
        }
        else
        {
            WriteLine($"Output: {output}");
        }
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            WriteLine($"Error calling gh.  Exit code is {process.ExitCode}");
            WriteLine($"Error: {error}");
            throw new InvalidOperationException("Error calling gh.");
        }

        return Task.CompletedTask;
    }

    private async Task<GithubProjectItemsResponse> GetItemsAsync(GitHubProject project)
    {
        var template = $"project item-list {project.Number} --owner {project.Owner.Login} --format json";

        // call gh to get the items
        var processStartInfo = new ProcessStartInfo("gh", template)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        string output = string.Empty;
        string error = string.Empty;

        using var process = new Process();

        process.StartInfo = processStartInfo;
        process.Start();
        output = await process.StandardOutput.ReadToEndAsync();
        error = await process.StandardError.ReadToEndAsync();
        if (!string.IsNullOrEmpty(error))
        {
            WriteLine($"Error: {error}");
        }
        else
        {
            WriteLine($"Output: {output}");
        }

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            WriteLine($"Error calling gh.  Exit code is {process.ExitCode}");
            WriteLine($"Error: {error}");
            throw new InvalidOperationException("Error calling gh.");
        }

        if (string.IsNullOrEmpty(output) == true)
        {
            WriteLine("No items returned from gh.");
        }

        var items = JsonSerializer.Deserialize<GithubProjectItemsResponse>(output);

        if (items == null)
        {
            throw new InvalidOperationException("Could not deserialize items.");
        }
        else
        {
            return items;
        }

    }

}

