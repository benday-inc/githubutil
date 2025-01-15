using Benday.CommandsFramework;
using Benday.GitHubUtil.Api.Messages.GetGraphQlMetadatas;
using Benday.GitHubUtil.Api.Messages.GetGraphQlTypeMetadatas;
using Benday.GitHubUtil.Api.Messages.ListProjectIterations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Benday.GitHubUtil.Api;

[Command(
    Category = Constants.Category_Projects,
    Name = Constants.CommandName_GraphQlMetadata,
        Description = "Get graphql metadata",
        IsAsync = true)]
public class GraphQlMetadataCommand : MetadataQueryCommand
{
    public GraphQlMetadataCommand(
        CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        var arguments = new ArgumentCollection();

        arguments.AddString("filter").AsNotRequired().
          WithDescription("Filter for the query.").
          WithDefaultValue(string.Empty);

        arguments.AddBoolean("dump").
            AsNotRequired().
            AllowEmptyValue().
            WithDescription("Dump the entire response to disk.").
            WithDefaultValue(false);

        return arguments;
    }

    protected override async Task OnExecute()
    {
        WriteLine("Getting metadata...");

        var filter = Arguments.GetStringValue("filter");


        var dump = Arguments.GetBooleanValue("dump");

        if (dump == false)
        {
            await GetMetadata(filter);
        }
        else
        {
            var currentDir = Environment.CurrentDirectory;

            var outputDir = Path.Combine("output", DateTime.Now.Ticks.ToString());

            if (Directory.Exists(outputDir) == false)
            {
                Directory.CreateDirectory(outputDir);
            }

            var result = await GetMetadata(string.Empty);

            WriteJsonToDisk(result, outputDir, "metadata.json");

            foreach (var type in result.Data.Schema.Types)
            {
                if (type.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) == true)
                {
                    WriteLine($"Getting metadata for {type.Name}...");

                    var typeResult = await GetTypeMetadata(type.Name, filter, true);

                    WriteJsonToDisk(typeResult, outputDir, $"type-{type.Name}.json");
                }
                else
                {
                    continue;
                }
            }
        }
    }
    
    private void WriteJsonToDisk(object result, string outputDir, string filename)
    {
        var outputFilename = Path.Combine(outputDir, filename);
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
        File.WriteAllText(outputFilename, json);
        WriteLine($"Wrote to {outputFilename}");
    }
}