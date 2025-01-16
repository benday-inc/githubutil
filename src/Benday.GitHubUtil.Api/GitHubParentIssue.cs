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

    /// <summary>
    /// Adds a child that's not part of the project
    /// </summary>
    /// <param name="subIssue"></param>
    public void AddChild(GetSubIssuesInfoResponse subIssue)
    {
        OtherChildren.Add(subIssue);
    }

    /// <summary>
    /// Adds a child that's part of the project
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void AddChild(Item item)
    {
        Children.Add(item);
    }

    /// <summary>
    /// The children that are not part of the project
    /// </summary>
    public List<GetSubIssuesInfoResponse> OtherChildren { get; } = new();

    /// <summary>
    /// The children that are not part of the project
    /// </summary>
    public List<Item> Children { get; } = new();
}