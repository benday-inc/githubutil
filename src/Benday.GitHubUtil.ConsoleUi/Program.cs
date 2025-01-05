using System.Diagnostics;
using System.Reflection;
using System.Text;
using Benday.CommandsFramework;
using Benday.GitHubUtil.Api;

namespace Benday.GitHubUtil.ConsoleUi;

class Program
{
    static void Main(string[] args)
    {
        var assembly = typeof(DeleteProjectItemsCommand).Assembly;

        var versionInfo =
            FileVersionInfo.GetVersionInfo(
                Assembly.GetExecutingAssembly().Location);

        var options = new DefaultProgramOptions();

        options.Version = $"v{versionInfo.FileVersion}";
        options.ApplicationName = "GitHub Utilities";
        options.Website = "https://www.benday.com";
        options.UsesConfiguration = false;

        var program = new DefaultProgram(options, assembly);

        program.Run(args);
    }
}