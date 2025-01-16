using Benday.CommandsFramework;
using Benday.GitHubUtil.Api.Messages;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api;

[Command(
    Category = Constants.Category_TestData,
    Name = Constants.CommandName_ProjectIssuesRandom,
        Description = "Create draft issues in a project using random data generator",
        IsAsync = true)]
public class CreateWorkItemInfoFromDataGeneratorCommand : AsynchronousCommand
{
    public CreateWorkItemInfoFromDataGeneratorCommand(
        CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        var arguments = new ArgumentCollection();

        arguments.AddBoolean("github").AllowEmptyValue().AsNotRequired().
            WithDescription("Generate as a github cli script.");

        arguments.AddBoolean("estimates").AllowEmptyValue().AsNotRequired().
            WithDescription("If in github mode, generate estimates using fibonnaci values.");

        arguments.AddString("projectname").AllowEmptyValue().AsNotRequired().WithDescription("Name of the project to use.");

        return arguments;
    }

    protected override async Task OnExecute()
    {
        var github = Arguments.GetBooleanValue("github");
        var estimates = Arguments.GetBooleanValue("estimates");
        var projectName = Arguments.GetStringValue("projectname");

        if (github == false && estimates == false)
        {
            var items = GenerateTitles();

            foreach (var item in items)
            {
                WriteLine(item);
            }

        }
        else
        {
            await GenerateForGitHub(projectName, estimates);
        }


    }

    private async Task GenerateForGitHub(string projectName, bool estimates)
    {
        var generator = new WorkItemScriptGenerator(false);

        var items = GenerateTitles();

        if (string.IsNullOrEmpty(projectName) == true)
        {
            throw new InvalidOperationException("Project name is required.");
        }

        var projectInfo = await GetProjectInfo(projectName);

        GitHubFieldInfo? fieldInfo = null;

        if (estimates == true)
        {
            fieldInfo = await GetFieldInfo(projectInfo);

            if (fieldInfo == null)
            {
                throw new InvalidOperationException("Could not get field info.");
            }
        }

        foreach (var item in items)
        {
            var title = item;
            await CreateItemInProject(projectInfo, estimates, generator, title, fieldInfo);
        }
        // var template = $"gh issue create --title \"Your Title\" --draft --project \"Your Project Name\"";
    }

    private async Task<GitHubFieldInfo?> GetFieldInfo(GitHubProject projectInfo)
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


    private async Task<GitHubProject> GetProjectInfo(string projectName)
    {
        WriteLine($"Getting project info for {projectName}...");

        var template = $"project list --owner benday-inc --format json";

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

        var projectInfo = JsonSerializer.Deserialize<GitHubProjectsResponse>(output) ??
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


    private async Task CreateItemInProject(
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


    private async Task SetEstimate(GitHubProject project, string id, string estimate, GitHubFieldInfo fieldInfo)
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

    private List<string> GenerateTitles()
    {
        var generator = new WorkItemScriptGenerator(false);

        var numberOfTitles = 40;

        var items = new List<string>();

        for (int i = 0; i < numberOfTitles; i++)
        {
            items.Add(generator.GetRandomTitle());
        }

        return items;
    }
}
