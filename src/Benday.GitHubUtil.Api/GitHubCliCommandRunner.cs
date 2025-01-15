using Benday.CommandsFramework;
using System.Diagnostics;
using System.Text;

namespace Benday.GitHubUtil.Api;

public class GitHubCliCommandRunner
{
    private readonly ITextOutputProvider _outputProvider;
    public GitHubCliCommandRunner(ITextOutputProvider outputProvider)
    {
        _outputProvider = outputProvider;
    }

    public string CommandName { get; set; } = string.Empty;
    public string SubCommandName { get; set; } = string.Empty;
    public string OutputText { get; set; } = string.Empty;
    public string ErrorText { get; set; } = string.Empty;
    public bool FormatJson { get; set; } = false;

    public List<GitHubCliArgument> Arguments { get; private set; } = new();

    public bool HasArguments
    {
        get
        {
            return Arguments.Count > 0;
        }
    }

    private void WriteLine(string message)
    {
        _outputProvider.WriteLine(message);
    }

    public bool IsSuccess { get; private set; }

    public async Task RunAsync()
    {
        if (string.IsNullOrEmpty(CommandName) == true)
        {
            throw new InvalidOperationException("CommandName is not set.");
        }

        var processStartInfo = GetProcessStartInfo();

        await RunCommandAsync(processStartInfo);

        // await RunCommandAsync2(processStartInfo);
    }

    private Task RunCommandAsync2(ProcessStartInfo processStartInfo)
    {
        processStartInfo.UseShellExecute = true;
        processStartInfo.RedirectStandardError = false;
        processStartInfo.RedirectStandardOutput = false;
        processStartInfo.CreateNoWindow = false;

        using var process = new Process();

        process.StartInfo = processStartInfo;

        process.Start();

        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            IsSuccess = true;
        }
        else
        {
            IsSuccess = false;
        }

        return Task.CompletedTask;
    }

    private Task RunCommandAsync(ProcessStartInfo processStartInfo)
    {
        var timeout = 10000;

        int exitCode = -1;

        var output = new StringBuilder();
        var error = new StringBuilder();

        using var process = new Process();

        process.StartInfo = processStartInfo;

        

        using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
        using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
        {
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    outputWaitHandle.Set();
                }
                else
                {
                    output.AppendLine(e.Data);
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    errorWaitHandle.Set();
                }
                else
                {
                    error.AppendLine(e.Data);
                }
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            if (process.WaitForExit(timeout)
                && outputWaitHandle.WaitOne(timeout)
                && errorWaitHandle.WaitOne(timeout))
            {
                exitCode = process.ExitCode;

                if (exitCode == 0)
                {
                    IsSuccess = true;
                }
                else
                {
                    IsSuccess = false;
                }
            }
            else
            {
                IsSuccess = false;
                // Timed out.
                throw new InvalidOperationException(
                    $"Process timed out after {timeout} milliseconds.");
            }
        }

        ErrorText = error.ToString();
        OutputText = output.ToString();

        return Task.CompletedTask;
    }

    private ProcessStartInfo GetProcessStartInfo()
    {
        var info = new ProcessStartInfo("gh")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (string.IsNullOrEmpty(CommandName) == false)
        {
            info.ArgumentList.Add(CommandName);

            if (string.IsNullOrEmpty(SubCommandName) == false)
            {
                info.ArgumentList.Add(SubCommandName);
            }
        }

        if (FormatJson == true)
        {
            info.ArgumentList.Add("--format");
            info.ArgumentList.Add("json");
        }

        foreach (var arg in Arguments)
        {
            if (arg.IsGraphQlFieldArgument == true)
            {
                info.ArgumentList.Add($"-F");

                if (arg.Name == "query")
                {
                    // get a path to a temp file
                    var tempFile = Path.GetTempFileName();

                    // write the query to the temp file
                    File.WriteAllText(tempFile, arg.Value);

                    // add the temp file to the argument list

                    info.ArgumentList.Add($"{arg.Name}=@{tempFile}");

                    // var valuePreparedForCommandLine = GetGraphQlQueryValueForCommandLine(arg.Value);
                    // var argValueQuoted = $"'{valuePreparedForCommandLine}'";
                    // var argValue = $"query={argValueQuoted}";

                    // info.ArgumentList.Add(argValue);
                }
                else
                {
                    info.ArgumentList.Add($"{arg.Name}={arg.Value}");
                }
            }
            else if (arg.HasValue == true)
            {
                info.ArgumentList.Add($"{arg.Name}");
                info.ArgumentList.Add(arg.Value);
            }
            else
            {
                info.ArgumentList.Add($"{arg.Name}");
            }
        }

        LogInfo(info);

        return info;
    }

    private void LogInfo(ProcessStartInfo info)
    {
        var builder = new StringBuilder();

        builder.Append(info.FileName);
        builder.Append(" ");

        foreach (var arg in info.ArgumentList)
        {
            builder.Append(arg);
            builder.Append(" ");
        }

        WriteLine("****");
        WriteLine(builder.ToString());
        WriteLine("****");
    }

    private string GetGraphQlQueryValueForCommandLine(string value)
    {
        using var reader = new StringReader(value);

        var lines = ReadAllLines(reader);

        var joinedLines = string.Join("\n", lines);

        return joinedLines.Trim();
    }

    private string[] ReadAllLines(StringReader reader)
    {
        var lines = new List<string>();

        var line = reader.ReadLine();

        while (line != null)
        {
            lines.Add(line);

            line = reader.ReadLine();
        }

        return lines.ToArray();
    }

    public void AddFieldArgument(string argName, string argValue)
    {
        Arguments.Add(new GitHubCliArgument(true, argName, argValue));
    }
}
