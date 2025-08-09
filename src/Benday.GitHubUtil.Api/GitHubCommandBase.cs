using System.Diagnostics;
using System.Text.Json;
using Benday.CommandsFramework;
using Benday.GitHubUtil.Api.Messages;

namespace Benday.GitHubUtil.Api;

public abstract class GitHubCommandBase : AsynchronousCommand
{
    protected GitHubCommandBase(CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
    {
    }

    protected async Task<GitHubFieldInfo?> GetFieldInfo(GitHubProject projectInfo)
    {
        WriteLine($"Getting field info for project {projectInfo.Title}...");

        var template = $"project field-list {projectInfo.Number} --owner {projectInfo.Owner.Login} --format json";

        var processStartInfo = new ProcessStartInfo("gh", template)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        string output = string.Empty;
        string error = string.Empty;

        using (var process = new Process())
        {
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
        }

        if (string.IsNullOrEmpty(output) == true)
        {
            throw new InvalidOperationException("No output from gh command.");
        }

        var fieldInfo = JsonSerializer.Deserialize<GithubProjectFieldInfoResponse>(output) ??
            throw new InvalidOperationException("Could not deserialize output.");

        var estimateField = fieldInfo.Fields.FirstOrDefault(
            f => f.Name.Equals("Estimate", StringComparison.OrdinalIgnoreCase));

        if (estimateField == null)
        {
            throw new InvalidOperationException("Could not find field with name Estimate.");
        }
        else
        {
            WriteLine($"Found field with id {estimateField.Id}");
            return estimateField;
        }
    }

    protected async Task CreateItemInProject(
            GitHubProject project, bool estimates, WorkItemScriptGenerator generator, string title,
            GitHubFieldInfo? fieldInfo)
    {
        if (estimates == true && fieldInfo == null)
        {
            throw new InvalidOperationException("Field info is required for estimates.");
        }

        WriteLine($"Creating item with title {title}...");

        // var template = $"gh project item-create 19 --owner benday-inc --title \"{title}\" --output json";
        var template = $"project item-create {project.Number} --owner {project.Owner.Login} --title \"{title}\" --format json";

        WriteLine($"calling... gh {template}");
        WriteLine();

        var processStartInfo = new ProcessStartInfo("gh", template)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        string output = string.Empty;
        string error = string.Empty;

        using (var process = new Process())
        {
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
        }

        if (string.IsNullOrEmpty(output) == true)
        {
            throw new InvalidOperationException("No output from gh command.");
        }

        var issueInfo = JsonSerializer.Deserialize<GitHubProjectIssueInfo>(output) ??
            throw new InvalidOperationException("Could not deserialize output.");

        WriteLine($"Created draft project issue id: {issueInfo.Id}");

        if (estimates == true)
        {
            if (fieldInfo == null)
            {
                throw new InvalidOperationException("Field info is required for estimates.");
            }

            var estimate = generator.GetRandomEstimate();

            await SetEstimate(project, issueInfo.Id, estimate, fieldInfo);
        }

        WriteLine(template);
    }

    protected async Task<GitHubProjectIssueInfo> CreateItemInProject(
            GitHubProject project, string title, string body)
    {
        WriteLine($"Creating item with title {title}...");

        // var template = $"gh project item-create 19 --owner benday-inc --title \"{title}\" --output json";
        var template = $"project item-create {project.Number} --owner {project.Owner.Login} --title \"{title}\" --body \"{body}\" --format json";

        WriteLine($"calling... gh {template}");
        WriteLine();

        var processStartInfo = new ProcessStartInfo("gh", template)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        string output = string.Empty;
        string error = string.Empty;

        using (var process = new Process())
        {
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
        }

        if (string.IsNullOrEmpty(output) == true)
        {
            throw new InvalidOperationException("No output from gh command.");
        }

        var issueInfo = JsonSerializer.Deserialize<GitHubProjectIssueInfo>(output) ??
            throw new InvalidOperationException("Could not deserialize output.");

        WriteLine($"Created draft project issue id: {issueInfo.Id}");

        return issueInfo;
    }


    protected async Task SetEstimate(GitHubProject project, string id, string estimate, GitHubFieldInfo fieldInfo)
    {
        WriteLine($"Setting estimate to {estimate} for id {id}...");

        var estimateFieldId = fieldInfo.Id;

        WriteLine($"Field id is {estimateFieldId}");

        var template = $"project item-edit --project-id {project.Id} --field-id {estimateFieldId} --id {id} --number {estimate}";

        var processStartInfo = new ProcessStartInfo("gh", template)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        string output = string.Empty;
        string error = string.Empty;

        WriteLine($"calling... gh {template}");



        using (var process = new Process())
        {
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

                WriteLine("Error code {process.ExitCode} from gh command.");
                WriteLine(error);

                throw new InvalidOperationException($"Error code {process.ExitCode} from gh command.");

            }
        }

        WriteLine($"Set estimate to {estimate} for id {id}");
        WriteLine();
    }


    protected async Task<GitHubProject> GetProjectInfo(string projectName, string ownerId)
    {
        WriteLine($"Getting project info for {projectName}...");

        var template = $"project list --owner {ownerId} --format json";

        var runner = new GitHubCliCommandRunner(_OutputProvider);

        runner.CommandName = "project";
        runner.SubCommandName = "list";

        runner.FormatJson = true;

        runner.AddArgument("--owner", ownerId);

        await runner.RunAsync();

        if (runner.IsSuccess == false)
        {
            throw new InvalidOperationException("Could not get project list.");
        }
        else
        {
            var projectInfo = JsonSerializer.Deserialize<GitHubProjectsResponse>(runner.OutputText) ??
            throw new InvalidOperationException("Could not deserialize output.");

            var project = projectInfo.Projects.FirstOrDefault(
                p => p.Title.Equals(projectName, StringComparison.OrdinalIgnoreCase));

            if (project == null)
            {
                throw new InvalidOperationException($"Could not find project with name {projectName}.");
            }
            else
            {
                WriteLine($"Found project with id {project.Id}");

                return project;
            }
        }
    }
}
