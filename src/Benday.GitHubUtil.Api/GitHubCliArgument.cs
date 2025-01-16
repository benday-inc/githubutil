namespace Benday.GitHubUtil.Api;

public class GitHubCliArgument
{
    public GitHubCliArgument(string name)
    {
        ArgumentType = GitHubCliArgumentType.Default;
        Name = name;
        HasValue = false;
        Value = string.Empty;
    }

    public GitHubCliArgument(string name, string value)
    {
        ArgumentType = GitHubCliArgumentType.Default;
        Name = name;
        HasValue = true;
        Value = value;
    }

    public GitHubCliArgument(GitHubCliArgumentType argType, string name, string value)
    {
        ArgumentType = argType;
        Name = name;
        HasValue = true;
        Value = value;
    }

    public string Name { get; set; }
    public bool HasValue { get; private set; }

    public GitHubCliArgumentType ArgumentType { get; set; }

    public string Value { get; set; }
}
