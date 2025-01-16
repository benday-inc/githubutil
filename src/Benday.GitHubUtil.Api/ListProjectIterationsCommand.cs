using Benday.CommandsFramework;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api;

[Command(
    Category = Constants.Category_Projects,
    Name = Constants.CommandName_ProjectIterations,
        Description = "Get a list of iterations for a project",
        IsAsync = true)]
public class ListProjectIterationsCommand : ProjectQueryCommand
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

        var response = await GetIterations(projectName, ownerId);

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
                    WriteLine($"    Is Current: {iteration.IsCurrentIteration}");
                }
            }
        }
    }

    
}
