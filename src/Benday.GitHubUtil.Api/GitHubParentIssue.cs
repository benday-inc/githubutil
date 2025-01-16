using Benday.GitHubUtil.Api.Messages.GetSubIssues;
using Benday.GitHubUtil.Api.Messages.ListProjectIssues;

namespace Benday.GitHubUtil.Api;

public class GitHubParentIssue
{
    public GitHubParentIssue(Item item)
    {
        Item = item;
    }

    public Item Item { get; }
    public void AddChild(GetSubIssuesInfoResponse subIssue)
    {
        Children.Add(subIssue);
    }

    public List<GetSubIssuesInfoResponse> Children { get; } = new();
}