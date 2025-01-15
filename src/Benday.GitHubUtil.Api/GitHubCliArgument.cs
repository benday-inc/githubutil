namespace Benday.GitHubUtil.Api;

public class GitHubCliArgument
    {
        public GitHubCliArgument(string name)
        {
            Name = name;
            HasValue = false;
            Value = string.Empty;
        }

        public GitHubCliArgument(string name, string value)
        {
            Name = name;
            HasValue = true;
            Value = value;
        }

        public GitHubCliArgument(bool isGraphQlFieldArgument, string name, string value)
        {
            if (isGraphQlFieldArgument == false)
            {
                throw new ArgumentException("isGraphQlFieldArgument must be true.");
            }
            
            IsGraphQlFieldArgument = isGraphQlFieldArgument;
            Name = name;
            HasValue = true;
            Value = value;
        }

        public string Name { get; set; }
        public bool HasValue { get; private set; }
        public bool IsGraphQlFieldArgument { get; private set; }
        public string Value { get; set; }
    }