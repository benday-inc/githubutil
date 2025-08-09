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
public class CreateProjectPBIsFromDataGeneratorCommand : GitHubCommandBase
{
    public CreateProjectPBIsFromDataGeneratorCommand(
        CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        var arguments = new ArgumentCollection();

        arguments.AddBoolean("estimates").AllowEmptyValue().AsNotRequired().
            WithDescription("Generate estimates using fibonnaci values.").
            WithDefaultValue(false);

        arguments.AddString(Constants.CommandArg_OwnerId).AsRequired().WithDescription("Owner id.");

        arguments.AddString(Constants.CommandArg_ProjectName).WithDescription("Name of the project to use.");

        arguments.AddInt32(Constants.CommandArg_Count).AsNotRequired().
            WithDescription("Number of items to create. (Default value is 10)").WithDefaultValue(10);

        return arguments;
    }

    protected override async Task OnExecute()
    {
        var estimates = Arguments.GetBooleanValue("estimates");
        var projectName = Arguments.GetStringValue("projectname");
        var ownerId = Arguments.GetStringValue("ownerid");
        var count = Arguments.GetInt32Value("count");

        await GenerateForGitHub(projectName, estimates, ownerId, count);
    }

    private async Task GenerateForGitHub(string projectName, bool estimates, string ownerId, int numberOfTitles)
    {
        var generator = new WorkItemScriptGenerator(false);

        var items = GenerateTitles(numberOfTitles);

        if (string.IsNullOrEmpty(projectName) == true)
        {
            throw new InvalidOperationException("Project name is required.");
        }

        var projectInfo = await GetProjectInfo(projectName, ownerId);

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

    

    private List<string> GenerateTitles(int numberOfTitles)
    {
        var generator = new WorkItemScriptGenerator(false);

        var items = new List<string>();

        for (int i = 0; i < numberOfTitles; i++)
        {
            items.Add(generator.GetRandomTitle());
        }

        return items;
    }
}
