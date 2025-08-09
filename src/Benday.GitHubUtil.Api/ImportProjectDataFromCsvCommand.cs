using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Benday.CommandsFramework;
using Benday.CommandsFramework.DataFormatting;
using Benday.GitHubUtil.Api.Messages;

namespace Benday.GitHubUtil.Api;

[Command(
    Category = Constants.Category_Projects,
    Name = Constants.CommandName_ImportProjectDataFromCsv,
        Description = "Import project data from a CSV file",
        IsAsync = true)]
public class ImportProjectDataFromCsvCommand : ProjectQueryCommand
{
    public ImportProjectDataFromCsvCommand(
        CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        var arguments = new ArgumentCollection();

        arguments.AddFile(Constants.CommandArg_InputFile)
            .MustExist()
            .AsRequired()
            .WithDescription("Path to the input CSV file.")
            .FromPositionalArgument(1)
            ;

        arguments.AddString("projectname").AsRequired().WithDescription("Name of the project to use.");
        arguments.AddString("ownerid").AsRequired().WithDescription("ID of the owner of the project.");

        return arguments;
    }

    protected override async Task OnExecute()
    {
        var projectName = Arguments.GetStringValue("projectname");
        var ownerId = Arguments.GetStringValue("ownerid");

        var inputFile = Arguments.GetPathToFile(Constants.CommandArg_InputFile);

        var projectInfo = await GetProjectInfo(projectName, ownerId);

        await ImportProjectDataFromCsv(inputFile, projectInfo);
    }

    private async Task ImportProjectDataFromCsv(string inputFile, GitHubProject projectInfo)
    {
        var content = await File.ReadAllTextAsync(inputFile);
        var csv = new CsvReader(content);

        var records = csv.ToList();

        if (!records.Any())
        {
            throw new KnownException("No valid records found.");
        }

        var expectedColumnNames = new[] { "Title", "Labels" };

        var columnNames = csv.GetColumnNames()?.ToList() ?? new List<string>();

        foreach (var item in expectedColumnNames)
        {
            if (!columnNames.Contains(item))
            {
                throw new KnownException($"CSV file is missing required column: {item}");
            }
        }

        foreach (var record in records)
        {
            await ProcessRecord(record, projectInfo);
        }
    }

    private async Task ProcessRecord(CsvRow record, GitHubProject projectInfo)
    {
        var title = record["Title"];
        var labels = record["Labels"]?.Split(',') ?? Array.Empty<string>();

        await CreateGitHubIssue(title, labels, projectInfo);
    }

    private async Task CreateGitHubIssue(string title, string[] labels, GitHubProject projectInfo)
    {
        WriteLine($"Creating issue with title: {title} and labels: ");
        foreach (var label in labels)
        {
            WriteLine($" - {label}");
        }

        var builder = new StringBuilder();
        builder.AppendLine($"Title: {title}");
        builder.AppendLine("Labels:");
        foreach (var label in labels)
        {
            builder.AppendLine($" - {label}");
        }

        var issueInfo = await CreateItemInProject(projectInfo, title, builder.ToString());

        WriteLine($"Created issue with ID: {issueInfo.Id}");

    }
}