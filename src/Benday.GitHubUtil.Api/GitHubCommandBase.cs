using Benday.CommandsFramework;
using Benday.GitHubUtil.Api.Messages;
using System.Diagnostics;
using System.Text.Json;

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
