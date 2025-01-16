using Benday.CommandsFramework;
using Benday.GitHubUtil.Api.Messages.GetGraphQlTypeMetadatas;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api;

[Command(
    Category = Constants.Category_Projects,
    Name = Constants.CommandName_GraphQlTypeMetadata,
        Description = "Get graphql metadata for a type",
        IsAsync = true)]
public class GraphQlTypeMetadataCommand : ProjectQueryCommand
{
    public GraphQlTypeMetadataCommand(
        CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        var arguments = new ArgumentCollection();

        arguments.AddString(Constants.CommandArg_TypeName).
          AsRequired().
          WithDescription("Name of the type to get metadata for.");

        arguments.AddString("filter").AsNotRequired().
          WithDescription("Filter for the query.").
          WithDefaultValue(string.Empty);

        return arguments;
    }

    protected override async Task OnExecute()
    {
        WriteLine("Getting metadata for type...");

        var typeName = Arguments.GetStringValue(Constants.CommandArg_TypeName);
        var filter = Arguments.GetStringValue("filter");
        var dump = Arguments.GetBooleanValue("dump");

        if (string.IsNullOrWhiteSpace(filter) == false)
        {
            WriteLine($"Filter: {filter}");
        }

        await GetTypeMetadata(typeName, filter);

    }
}